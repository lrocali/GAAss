using UnityEngine;
using System.Collections;

public class RoofTrigger : MonoBehaviour {

	public int buildingNumber;
	private GameObject roof;

	public int reduceRate =4;
	PlayerAwareness pa;
	float LastTime=0f;
	public float rejuvTime = 1f;

	// Use this for initialization
	void Start () {
		roof = GameObject.Find("Roof"+buildingNumber);
		pa = GameObject.Find("Player").GetComponent<PlayerAwareness>();
	}
	


	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag =="Player")
		{

			Vector3 newPos = roof.transform.position;
			newPos.y = 26f;
			roof.transform.position=newPos;
		}



	}

	void OnTriggerStay (Collider other)
	{

		if (other.gameObject.tag =="Player")
		{
			LastTime += Time.deltaTime;
			if (LastTime >rejuvTime)
			{
				pa.LoseAwareness(reduceRate);
				LastTime=0f;
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag =="Player")
		{

			Vector3 newPos = roof.transform.position;
			newPos.y = 4.5f;
			roof.transform.position=newPos;
		}

	}

}
