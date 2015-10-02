using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

	//player variables
	public Transform player;
	protected float distanceToPlayer;
	protected Vector3 targetPosition;
	protected Vector3 steerForce;
	[SerializeField]
	protected Vector3 velocity;
	protected float gravity = 10f;

	//steering behaviour variables
	public float arriveRadius;
	public float maxForce;
	public float maxRunningSpeed;
	public float mass;
	public Vector3 acceleration;
	public int rotateSpeed;
	public float arriveDamping;
	public float separationDistance = 15;
	public float separationFromOtherBGForce = 20;
	public float separationFromPlayerForce = 15;

	[SerializeField]
	int RegroupDistance;
	
	public float minDistanceToPlayer = 6;

	protected CharacterController controller;



	protected void Truncate (ref Vector3 myVector, float myMax)//not above max
	{
		if (myVector.magnitude > myMax) {
			myVector.Normalize ();// Vector3.normalized returns this vector with a magnitude of 1
			myVector *= myMax;//scale to max
		}
	}



}
