using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BodyGuardFollow : Follow
{
	enum BGState
	{
		Follow,
		Idle,
		Protect,
		ProtectFromShoot
	};

	private BGState bgstate;

	//bodyguard variables
	public int numberOfBG;
	public float minDistanceOfPPToPlayer = 15;
	GameObject[] bodyguards;
	float vlctToProtectFromShoot = 50f;
	//public int minDistanceToPlayer = 5;
	public float avoidDistance;

	//paparazzi variables
	GameObject[] paparazzis;
	int numberOfPP;

	//paparazzis who are closer than the min distance to player
	public List<int> dangerousPPIndex = new List<int>();	

	//gizmos
	public bool DrawGizmos;

	//awareness 
	PlayerAwareness pa;
	public float AwIncrease=1f;

    Vector3 initialPosition;

	// Use this for initialization
	void Start ()
	{
        initialPosition = transform.position;

		player = GameObject.Find ("Player").transform;

		controller = GetComponent<CharacterController> ();

		bodyguards = GameObject.FindGameObjectsWithTag ("BG");
		paparazzis = GameObject.FindGameObjectsWithTag ("PP");

		bgstate = BGState.Idle;

		orderBGs ();

		numberOfBG = bodyguards.Length;

		numberOfPP = paparazzis.Length;

		pa = GameObject.Find("Player").GetComponent<PlayerAwareness>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		distanceToPlayer = Vector3.Distance (this.transform.position, player.position);
		targetPosition = player.position;

		HandleState ();

		steerForce = Vector3.zero;//start from scratch

		//update the list of paparazzis who are inside the "dangerous area"
		dangerousPPIndex.Clear ();
		for (int i = 0; i < numberOfPP; i++) {
			float dist = Vector3.Distance (player.position, paparazzis [i].transform.position);
			
			if (dist < minDistanceOfPPToPlayer) {
				dangerousPPIndex.Add(i);
			}
		} 

		ProcessMovement ();
	
	}

	void HandleState ()
	{


		if (distanceToPlayer > minDistanceToPlayer) {
			bgstate = BGState.Follow;
		} else {
			bgstate = BGState.Protect;
		}

		if (dangerousPPIndex.Count > 0/* && Input.GetKey(KeyCode.P)*/) {
			bgstate = BGState.ProtectFromShoot;
			pa.AddAwareness(AwIncrease*Time.deltaTime*0.1f);


		}

	}

	void ProcessMovement ()
	{
		if (bgstate == BGState.Follow) {
			steerForce = Arrive (player.position);
			steerForce += Separation ();
			steerForce += AvoidAbstacle(steerForce);

		}
		//protect state
		if (bgstate == BGState.Protect) {
			steerForce = Arrive (player.position);
			steerForce += SeparationToProtect ();
			steerForce += separationFromPlayer ();
			steerForce += AvoidAbstacle(SeparationToProtect ());
		}
		//RegroupToProtect state
		if (bgstate == BGState.ProtectFromShoot) {
			steerForce = Arrive (player.position);
			//steerForce 
			steerForce += Separation ();
			steerForce += RegroupToProtect();
			//steerForce += SeparationToProtect ();
			steerForce += separationFromPlayer ();
		
		}


		if (bgstate == BGState.ProtectFromShoot) {
			Truncate (ref steerForce, 80);// not > max
		} else {
			Truncate (ref steerForce, maxForce);// not > max
		}


		acceleration = steerForce / mass;
		velocity += acceleration;//velocity = transform.TransformDirection(velocity);
		Truncate (ref velocity, maxRunningSpeed);// not > max

		if (controller.isGrounded) {
			controller.Move (velocity * Time.deltaTime);//move
			
		} else {
			controller.Move (new Vector3 (0, -gravity * Time.deltaTime, 0));//fall down
		}
			
		//rotate
		if (new Vector3 (velocity.x, 0, velocity.z) != Vector3.zero) {//otherwise warning
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (new Vector3 (velocity.x, 0, velocity.z)), rotateSpeed * Time.deltaTime);
		}
		//}
	}



	public Vector3 Separation ()
	{
		Vector3 mySteeringForce = Vector3.zero;

		int protectFactor = 0;
		if (bgstate == BGState.ProtectFromShoot) {
			protectFactor = 2;
		}
		for (int i = 0; i < numberOfBG; i++) {
			float dist = Vector3.Distance (transform.position, bodyguards [i].transform.position);
			//add a force away for each other BG ~ 1/r, far away bodyguards yield small force
			if (dist < (separationDistance - protectFactor) && dist != 0 && bgstate != BGState.Protect) {//close enough to take into account, but not 0, because we will divide by it, dist = 0 means this GO
				//seperationForce was 5;
				mySteeringForce += (transform.position - bodyguards [i].transform.position).normalized * separationFromOtherBGForce / dist;
			}
		}    

		
		Debug.DrawRay (transform.position, mySteeringForce, Color.cyan);
		
		return mySteeringForce;
	}

	//Makes bodyguard stay from a certain distance from the player
	public Vector3 separationFromPlayer() {
		Vector3 mySteeringForce = Vector3.zero;
		//Makes bodyguard stay from a certain distance from the player
		float dist = Vector3.Distance (transform.position, player.transform.position);
		if (dist < minDistanceToPlayer - 1) {
			mySteeringForce += (transform.position - player.transform.position) * separationFromPlayerForce ;
			Debug.DrawRay (transform.position, mySteeringForce, Color.white);

		}

		return mySteeringForce;
	}

	public Vector3 SeparationToProtect ()
	{
		Vector3 mySteeringForce = Vector3.zero;
		int bgi = GetBGIndex (this.transform.gameObject.name);
		int i = 0;
		float minDistance = 20;
		for (i = 0; i < numberOfBG; i++) {
			float distance = Vector3.Distance (bodyguards [i].transform.position, bodyguards [i].transform.position);

			if (distance < minDistance) {
				minDistance = distance;
			}
		}
		if (!isOnInterval(minDistance,6.7f,7.3f)) {
			float posAngRad = 2 * Mathf.PI * bgi / 3;
			Vector3 protcPos = new Vector3 (minDistanceToPlayer * Mathf.Cos (posAngRad), 0, minDistanceToPlayer * Mathf.Sin (posAngRad));
			mySteeringForce = player.position - transform.position + protcPos;
			float distanceToProtcPos = Vector3.Distance (transform.position, protcPos);
			mySteeringForce = mySteeringForce * distanceToProtcPos/10;
				
			//Debug.DrawRay (transform.position, mySteeringForce, Color.red);
		}




		return mySteeringForce;
	}

	public Vector3 RegroupToProtect()
	{
		Vector3 mySteeringForce = Vector3.zero;
		int bgi = GetBGIndex (this.transform.gameObject.name);
		int closestBG; //removed unused variable i
		bool protecting = false;	//protecting direct from some paparazzi

		//for each paparazzi in the dangerous area the body guard will check if he is the closes from someone
		foreach (int index in dangerousPPIndex) {

			closestBG = getClosestBGFromPaparazi (index);

			//if the current bodyguard is the closes from some paparazzi he will protect this paparazzi directly
			if (bgi == closestBG && !protecting) { 

				protecting = true;
				protectFromPaparazzi (index, true);

				Debug.DrawLine (transform.position, paparazzis [index].transform.position, Color.blue);

			}
		}

		//if the current bg is not the closest from any pp but there is one bg protecting from more than one paparazzi the bg will help
		if (!protecting && checkClosests ()) {
			int numOfDangerousPP = dangerousPPIndex.Count;
			//By defaul the bg will help to protect the last dangerous paparazzi (think for a better solution)
			protectFromPaparazzi (dangerousPPIndex [numOfDangerousPP - 1], false);

			Debug.DrawLine (transform.position, paparazzis [dangerousPPIndex [numOfDangerousPP - 1]].transform.position, Color.red);

		}/* else if (!protecting){
			//mySteeringForce = SeparationToProtect ();
			//mySteeringForce += separationFromPlayer ();
		}*/

		/*
		//Makes bodyguard stay from a certain distance from the player
		float dist = Vector3.Distance (transform.position, player.transform.position);
		mySteeringForce += (transform.position - player.transform.position).normalized * separationFromPlayerForce / dist;
		
		Debug.DrawRay (transform.position, mySteeringForce, Color.white);*/
		return mySteeringForce;
	}

	//function to rotate bodyguards around the player to protect from specific bodyguard
	public void protectFromPaparazzi(int index, bool protectOrHelp) {

		//get angle of the current bg and the paparazzi in relation to the player
		float angOfBG = GetAngle (player.position, transform.position);
		float angOfPP = GetAngle (player.position, paparazzis [index].transform.position);

		//the current bg can go to protect directly or help another bg who is already protecting
		float distanceFromTargetAngle;
		if (protectOrHelp) {
			distanceFromTargetAngle = 0.1f;
		} else {
			distanceFromTargetAngle = 0.7f;
		}

		//if the difference of his angle and the target angle in relation to the player is greatter than some amount they will move to that target angle
		if (!diferenceLessThan (angOfBG, angOfPP, distanceFromTargetAngle)) {

			//To decide in which side rotate the bodyguard
			float difference = angOfBG - angOfPP;
			//print(di
			if (difference < 0) {
				if (Mod(difference) < Mathf.PI)
					transform.RotateAround (player.position, Vector3.down, vlctToProtectFromShoot * Time.deltaTime);
				else 
					transform.RotateAround (player.position, Vector3.up, vlctToProtectFromShoot * Time.deltaTime);
			} else {
				if (Mod(difference) < Mathf.PI)
					transform.RotateAround (player.position, Vector3.up, vlctToProtectFromShoot * Time.deltaTime);
				else 
					transform.RotateAround (player.position, Vector3.down, vlctToProtectFromShoot * Time.deltaTime);
				
				
			}
		}
	}


	public float Mod(float x) {
		if (x < 0)
			x *= (-1);
		return x;
	}

	//Check if there are one bodyguard closer to more than one paparazzi
	public bool checkClosests() {
		int i;
		int flag;
		int closestBG;
		for (i=0; i<numberOfBG; i++) {
			flag = 0;
			foreach (int index in dangerousPPIndex) {
				closestBG = getClosestBGFromPaparazi (index);
				if (closestBG == i) {
					if (flag == 1) {
						return true;
					}
					flag = 1;
				}
			}
		}
			return false;
	}

	//return the index of the closes bodygurd from a specific paparazzi
	public int getClosestBGFromPaparazi(int paparazziIndex) {
		int i, indexOfClosest = 0;

		float minDistance = minDistanceOfPPToPlayer;
		for (i = 0; i < numberOfBG; i++) {
			float distance;
			distance = Vector3.Distance (bodyguards [i].transform.position, paparazzis[paparazziIndex].transform.position);
		
			if (distance < minDistance) {
				minDistance = distance;
				indexOfClosest = i;
			}
		}
		return indexOfClosest;
	}

	//get position of paparazzi (or any object) in relation to the player
	public float GetAngle(Vector3 player, Vector3 paparazzi) {
		Vector3 vectorPlayerPaparazzi = paparazzi - player;
		float angle = Vector3.Angle (vectorPlayerPaparazzi, new Vector3 (1, 0, 0));

		if (vectorPlayerPaparazzi.z < 0)
			angle = 360 - angle;
		return angle*Mathf.PI/180;
	}



	//return the index of the bg via his name
	private int GetBGIndex (string bdn)
	{
		if (bdn == "BG0") {
			return 0;
		}
		if (bdn == "BG1") {
			return 1;
		}
		if (bdn == "BG2") {
			return 2;
		}

		return 0;
	}
	public Vector3 AvoidAbstacle(Vector3 arrivePosition) {
		Vector3 mySteeringForce = Vector3.zero;

		Vector3 ahead = player.position - transform.position;
		RaycastHit hit;
	
		ahead = ahead.normalized;

		Vector3 up =  new Vector3(0.0f, 1.0f, 0.0f);

		Vector3 right = Vector3.Cross(ahead.normalized, up.normalized);
		Vector3 left = -right;

		ahead *= avoidDistance;
		left *= avoidDistance/2f;
		right *= avoidDistance/2f;

		Vector3 [] directions = {ahead,left,right};

		Debug.DrawRay (transform.position, ahead, Color.yellow);
		Debug.DrawRay (transform.position, left, Color.yellow);
		Debug.DrawRay (transform.position, right, Color.yellow);

		float scaleFactor = 0;	//for scaling the steering

		foreach (Vector3 direction in directions) {

			if (Physics.Raycast (transform.position, direction, out hit, avoidDistance)) {
				if (hit.transform.tag != "BG" && hit.transform.tag != "PP" && hit.transform != player.transform) {
			
					float distanceToTarget = Vector3.Distance (hit.transform.position, transform.position);
					scaleFactor = maxForce * (avoidDistance - distanceToTarget);
					//Debug.DrawLine (transform.position, -ahead, Color.yellow);
					//Debug.DrawRay (transform.position, direction, Color.green);
					mySteeringForce = direction;
					mySteeringForce *= scaleFactor;
					Debug.DrawRay (transform.position, mySteeringForce.normalized*4f, Color.red);
				}
			} 
		}
		return mySteeringForce;

	}
	public Vector3 Arrive (Vector3 arrivePosition)
	{
		float distanceToTarget = Vector3.Distance (arrivePosition, transform.position);//calc distance
		float scaleFactor = 0;//for scaling the steering
		Vector3 mySteeringForce;//to return
		int protectFactor = 0;
		if (bgstate == BGState.ProtectFromShoot) {
			protectFactor = 1;
		}
		if (distanceToTarget > arriveRadius+protectFactor) {//decrease acceleration
			scaleFactor = maxForce * ((distanceToTarget - arriveRadius) / distanceToTarget);   
		} else {//come to halt
			scaleFactor = 0;//no more accelerations
			velocity = Vector3.Lerp (velocity, Vector3.zero, Time.deltaTime * arriveDamping);//go to zero in some time
		}
		mySteeringForce = (arrivePosition - transform.position)/*.normalized*/ * scaleFactor;///look at target direction, normalized and scaled
		

		if (bgstate == BGState.Follow) {
			Debug.DrawLine (transform.position, arrivePosition, Color.blue);
		} else if (bgstate == BGState.Protect) {
			Debug.DrawLine (transform.position, arrivePosition, Color.green);
		} else {
			Debug.DrawLine (transform.position, arrivePosition, Color.yellow);
		}

		return mySteeringForce;
	}

	//order bgs
	public void orderBGs() {
		bodyguards [0] = GameObject.Find("BG0");
		bodyguards [1] = GameObject.Find ("BG1");
		bodyguards [2] = GameObject.Find ("BG2");
	}

	//chech if some number n is inside the interval ( a < n < b )
	public bool isOnInterval(float n, float a, float b) {
		if (n > a && b > n) {
			return true;
		}
		return false;
	}

	//check is the diference of a and b is less tang the maxDiff ( |a - b| < maxDiff )
	public bool diferenceLessThan(float a, float b, float maxDiff) {
		float diff = a - b;
		if (diff < 0) 
			diff *= (-1);
		if (diff < maxDiff) 
			return true;
		return false;
	}


	void OnDrawGizmos() {
		if (DrawGizmos)
		{
			/*
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (player.position, minDistanceToPlayer - 1);
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere (player.position, minDistanceToPlayer);
			Gizmos.color = Color.gray;
			Gizmos.DrawWireSphere (player.position, minDistanceOfPPToPlayer);
*/


		}
	}

    public void ResetPositions()
    {
        transform.position = initialPosition;
        bgstate = BGState.Idle;
    }
       



}
