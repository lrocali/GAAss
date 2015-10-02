using UnityEngine;
using System.Collections;

public class FollowSG : Follow {

	public GameObject [] sgs; //other school girls in the group
    Vector3 initalPosition;

	// Use this for initialization
	void Start () {
        initalPosition = transform.position;
		controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		//player in this case is always the lead school girl target
		distanceToPlayer = Vector3.Distance (this.transform.position, player.position);
		targetPosition = player.position;
		
		//steerForce = Vector3.zero;//start from scratch
		steerForce = Arrive (player.position);
		//steerForce += Separation ();
		//Debug.Log ("SG SF: "+steerForce);
		Truncate (ref steerForce, maxForce);// not > max
		acceleration = steerForce ; // /mass
		velocity += acceleration;//velocity = transform.TransformDirection(velocity);
		Truncate (ref velocity, maxRunningSpeed);// not > max
		
		
		if (controller.isGrounded) {
			controller.Move (velocity * Time.deltaTime);//move
			
		} else {
			controller.Move (new Vector3 (0, -gravity * Time.deltaTime, 0));//fall down
		}
	}

	public Vector3 Separation ()
	{
		Vector3 mySteeringForce = Vector3.zero;
		for (int i = 0; i < sgs.Length; i++) {
			float dist = Vector3.Distance (transform.position, sgs [i].transform.position);
			//add a force away for each other BG ~ 1/r, far away sgs yield small force
			if (dist < separationDistance && dist != 0) {//close enough to take into account, but not 0, because we will divide by it, dist = 0 means this GO
				//seperationForce was 5;
				mySteeringForce += (transform.position - sgs [i].transform.position).normalized * separationFromOtherBGForce / dist;
			}
		}    
		
		Debug.DrawRay (transform.position, mySteeringForce, Color.cyan);
		
		return mySteeringForce;
	}
	public Vector3 Arrive (Vector3 arrivePosition)
	{
		float distanceToTarget = Vector3.Distance (arrivePosition, transform.position);//calc distance

		//Debug.Log ("DTT: "+distanceToTarget);
		float scaleFactor = 1;//for scaling the steering
		Vector3 mySteeringForce;//to return
		if (distanceToTarget > arriveRadius) {//decrease acceleration
			scaleFactor = maxForce * ((distanceToTarget - arriveRadius) / distanceToTarget);   
		} else {//come to halt
			scaleFactor = 0;//no more accelerations
			velocity = Vector3.Lerp (velocity, Vector3.zero, Time.deltaTime * arriveDamping);//go to zero in some time
		}
		mySteeringForce = (arrivePosition - transform.position)/*.normalized*/ * scaleFactor;///look at target direction, normalized and scaled
		
		/*
		 * if (bgstate == BGState.Follow) {
				Debug.DrawLine (transform.position, arrivePosition, Color.blue);
			} else {
				Debug.DrawLine (transform.position, arrivePosition, Color.green);
			}
			*/
		
		return mySteeringForce;
	}

    public void ResetPositions()
    {
        transform.position = initalPosition;
    }
}
