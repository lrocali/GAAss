using UnityEngine;
using System.Collections;

public class UnitSG : Unit {

	enum SGState { Follow, Task };
	
	[SerializeField]
	SGState sgstate;
	float LastChecked =0f;
	Vector3 LastPosition;

	[SerializeField]
	float Awareness;

    Vector3 initialPosition;

	// Use this for initialization
	void Awake () {
		sgstate = SGState.Task;
        initialPosition = transform.position;

        pf = GameObject.Find("PathfindingManager").GetComponent<Pathfinding>();

		player = GameObject.Find("Player").transform;
		//AssignNewTaskLocation();
	}

	void Start() {
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	
	// Update is called once per frame
	void Update () {
		Awareness = player.GetComponent<PlayerAwareness>().GetAwareness();
		LastChecked += Time.deltaTime;
			
			if (LastChecked > 10f)
			{
				if (Vector3.Distance(LastPosition, transform.position)<2)
				{
				if (pf.TestIfPosWalkable(transform.position))
				{
					Debug.Log (gameObject.name +" was stuck.. going home! ");
					AssignNewTaskLocation();
				} 
				else
				{
					Debug.Log ("try push... "+transform.name);
					if (targetIndex < path.Length && targetIndex >0)
					{
						transform.position = Vector3.MoveTowards(transform.position,path[targetIndex-1],speed );
						Debug.Log ("Pushed! "+transform.name);
					}
					
				}
					
				}
				LastPosition = transform.position;
				LastChecked=0f;
			}

	}

	void  AssignNewTaskLocation ()
	{
		GameObject [] Locations = GameObject.FindGameObjectsWithTag("BTarget");
		int from=1000;
		for (int i=0; i<Locations.Length; i++)
		{
			if (Vector3.Distance (Locations[i].transform.position, target.position) <2f )
				from = i;
		}

		// ensure not same location
		int random = UnityEngine.Random.Range (0, Locations.Length-1);
		while (random == from)
		{
			random = UnityEngine.Random.Range (0, Locations.Length-1);
		}

	//	Debug.Log ("Returning " +Locations[random].transform.name +"next pos");
		target = Locations[random].transform;
		RecalculatePath();
	}

	override protected IEnumerator FollowPath() 
	{
	//	Debug.Log (this.transform.name +" start to follow path to " + target.name +": "+ target.position);
		
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
					if (sgstate == SGState.Task)
					{
						AssignNewTaskLocation();
						//RecalculatePath ();
						//yield break; //only breaks out when creates new path

					}
					else if (sgstate == SGState.Follow)
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
		
		switch (sgstate)
		{
			
		case SGState.Follow:
				
				if (distanceToPlayer < maxDistance && pf.TestIfPosWalkable(transform.position))
					
				{
					currentWaypoint = transform.position;
					targetIndex = path.Length; //force it to recalc new path
					// dont want to call recalc new path every update
				} 
				else if (distanceToPlayer > Awareness )
				{
					sgstate = SGState.Task;
					AssignNewTaskLocation();
				}

				else
				{
					//move toward next waypoint
					transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
					

					
				}
			
			//dont check if moved until they have reached their current goal
			break;
			
		case SGState.Task:

			transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);

			//check if within awareness circle, if yes change to follow!
			if (distanceToPlayer < Awareness && pf.TestIfPosWalkable(transform.position))
				
			{
				sgstate = SGState.Follow;
				target = player;
				RecalculatePath();
			}


			break;
			
		default:
			Debug.LogError ("Debug Me !!");
			break;
			
		}
		
		yield return null;
		}	
	}

    public void ResetPositions()
    {
        transform.position = initialPosition;
        sgstate = SGState.Task;
        RecalculatePath();
    }
}
