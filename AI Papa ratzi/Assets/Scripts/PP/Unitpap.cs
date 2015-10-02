using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Unitpap : Unit {


	//Rigidbody rb;

	public int numPhotos = 3;


	bool CanTakePhoto = true;


	enum PPState { Follow, TakePhoto, BackToCharge, Wait };

	[SerializeField]
	PPState ppstate;

	Animator anim;
	SpriteController sc;

    Vector3 initialPosition;


	//slider 
	Slider slider;
	float duration = 1.5f; //time to take photo

	float smoothness = 0.1f; // slider increments

	Vector3 LastPosition;
	float LastChecked=0f;

	void Awake ()
	{
		ppstate = PPState.Follow;
        pf = GameObject.Find("PathfindingManager").GetComponent<Pathfinding>();
        //will always start with the player as the target
        player = GameObject.Find("Player").transform;
		target = player;
        initialPosition = transform.position;
        slider = GetComponentInChildren<Slider>();

		//rb = GetComponent<Rigidbody>();

		anim = GetComponentInChildren<Animator>();
		sc = GetComponentInChildren <SpriteController>();
		
	}

	void Start() {
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	// Override Unit method
	
	override protected IEnumerator FollowPath() 
	{
		//Debug.Log (this.transform.name +" start to follow path to " + target.name +": "+ target.position);

		if (path.Length ==0)
		{
			//Debug.Log ("FollowPath broken");
			RecalculatePath ();
			yield return null; //onpath found will exit this coroutine anyway
		}

		Vector3 currentWaypoint = path[0];

		targetIndex=0;

		while (true) {

			//increment waypoint if reached current target waypoint
			if (transform.position == currentWaypoint) 
			{
				targetIndex ++;
				if (targetIndex >= path.Length) 
				{
					if (ppstate == PPState.BackToCharge)
					{
						StartCoroutine("WaitAtBase");
						anim.SetInteger("hSpeedInt", 0);

						if (pf.TestIfPosWalkable(player.transform.position) &&
						    ppstate != PPState.Wait) // if player is on walkable area, 
							// otherwise it will keep looping back until the player is in a walkable area
						{
							target = player;
							ppstate = PPState.Follow;
							RecalculatePath ();
							yield break; //only breaks out when creates new path
						}
					}
					else if (ppstate == PPState.Follow)
					{
						if (HasTargetMoved() && pf.TestIfPosWalkable(target.position))
						{
							
							RecalculatePath ();
						}
					}
					//yield break; //original position
				}
				if (targetIndex < path.Length) 
					currentWaypoint = path[targetIndex];
			}

			distanceToPlayer = Vector3.Distance(this.transform.position, player.position);

			switch (ppstate)
			{

			case PPState.Follow:

				if (distanceToPlayer < maxDistance && pf.TestIfPosWalkable(transform.position))

				{
					anim.SetInteger("hSpeedInt", 0);
					ppstate = PPState.TakePhoto;
				} 
				else
				{
					//move toward next waypoint
					transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);

					HandleSprite (currentWaypoint);

				}

				//dont check if moved until they have reached their current goal
				break;

			case PPState.TakePhoto:

				if (distanceToPlayer < maxDistance)
				{
					anim.SetBool("Shoot", true);
					if (numPhotos>-2)
					{
						if (CanTakePhoto)
						{
							if (numPhotos!= 3 && numPhotos != -1)
								GetComponent<PaparazziShoot>().ShootBullet();


							TakePhoto();
							StartCoroutine("PhotoCounter");
						}
					}
					else  // else has run out of photos and should go back to charge
					{
						anim.SetBool("Shoot", false);
						anim.SetInteger("hSpeedInt", 2);
						ppstate = PPState.BackToCharge;

						GetNewBattery(); //will give new target and recalc path

						yield break; // exit out of the while loop!
						// although OnPathFound will stop current coroutine.. 
					}
				} 
				else // target has moved out of distance
				{
					if (pf.TestIfPosWalkable(player.position))
					{
						Debug.Log (transform.name+ " target moved");
					        
						anim.SetBool("Shoot", false);
						ppstate = PPState.Follow;
						RecalculatePath (); // 
					}
				}
				break;

			case PPState.BackToCharge:

				if (distanceToPlayer < maxDistance && pf.TestIfPosWalkable(transform.position) 
				    && numPhotos >1 )
					
				{
					target = player;
					anim.SetInteger("hSpeedInt", 0);
					ppstate = PPState.TakePhoto;
				} 

				//move toward next waypoint until back to base
				transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
				HandleSprite (currentWaypoint);


				break;

			case PPState.Wait:

				break;

			default:
				Debug.LogError ("Debug Me !!");
				break;

			}

			yield return null;

		}
	}

	//only called when walking
	void HandleSprite (Vector3 waypoint)
	{
		//sprite handling
		anim.SetInteger("hSpeedInt", 2);
		Vector3 ToMove = transform.position-waypoint;
		if (ToMove.x <=0)
			sc.FaceRight (true);
		else
			sc.FaceRight(false);
	}
	

	IEnumerator PhotoCounter ()
	{

		float progress =0;
		float increment = smoothness/duration;

		while (progress < duration)
		{
			progress += increment;
			slider.value = progress;
			yield return new WaitForSeconds(smoothness);
		}

		CanTakePhoto = true;
		yield break;
	}

	IEnumerator WaitAtBase ()
	{
		ppstate = PPState.Wait;

		float WaitDuration = UnityEngine.Random.Range(2,10);
		float progress =0;
		float increment = smoothness/WaitDuration;
		
		while (progress < WaitDuration)
		{
			progress += increment;
			slider.value = progress;
			yield return new WaitForSeconds(smoothness);
		}

		while (!pf.TestIfPosWalkable(target.position))
		{
			yield return new WaitForSeconds(smoothness);
		}

		if (progress >= WaitDuration)
		{


			target = player;
			ppstate = PPState.Follow;
			//restore back to 3 photos
			numPhotos = 3;
			RecalculatePath();
		}

        yield break;
	}

	void Update ()
	{
	

		LastChecked += Time.deltaTime;

		if (LastChecked > 10f)
		{
			if (Vector3.Distance(LastPosition, transform.position)<2)
			{
				Debug.Log (gameObject.name +" was stuck! ");

				if (pf.TestIfPosWalkable(transform.position))
				{
					if (ppstate == PPState.Follow)
					{
					
						GetNewBattery();

					} else if (ppstate == PPState.BackToCharge)
					{
						// they are stuck at the base
						target=player;
						ppstate = PPState.Follow; //recently added
						RecalculatePath();
					} else if (ppstate == PPState.Wait)
					{
						target = player;
						ppstate = PPState.Follow;
						RecalculatePath();
					}
				}
				else
				{
					Debug.Log ("try push... "+transform.name);
					if (targetIndex <= path.Length && targetIndex >0)
					{
						transform.position = Vector3.MoveTowards(transform.position,path[targetIndex-1],speed );
						Debug.Log ("Pushed! "+transform.name);
					}
					else
					{
						transform.position = Vector3.MoveTowards(transform.position,Vector3.right,speed );
						Debug.Log ("Pushed Right! "+transform.name);
					}
				}

			}
			LastPosition = transform.position;
			LastChecked=0f;
		}

	}





	void TakePhoto ()
	{
		CanTakePhoto =false;
		slider.value =0;
		numPhotos--;
//		Debug.Log ("Take Photo ");
	}

	void GetNewBattery ()
	{
		ppstate = PPState.BackToCharge;
		Debug.Log (transform.name + " Get new battery ");
        // new target, choose closest 'battery store' transform and go there, then recalculate path...


        // go to closest one
        //int random = Random.Range(0, RechargeSpots.Length);
        target = GetClosestBatteryLocation();

        StopCoroutine("FollowPath");

		RecalculatePath ();
	}

    Transform GetClosestBatteryLocation()
    {
        GameObject[] RechargeSpots = GameObject.FindGameObjectsWithTag("Recharge");
        float closest = 5000f;
        Transform closestT = GameObject.FindGameObjectWithTag("Recharge").transform;

        //Transform closestRS; //removed as unused
        foreach (GameObject RS in RechargeSpots)
        {
            if (Vector3.Distance(this.transform.position, RS.transform.position) < closest)
            {
                closest = Vector3.Distance(this.transform.position, RS.transform.position);
                closestT = RS.transform;
            }
        }

        return closestT;
    }

    public void ResetPositions()
    {
        numPhotos = 3;

        ppstate = PPState.Follow;
        CanTakePhoto = true;

        transform.position = initialPosition;
        RecalculatePath();

    }


}
