using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	private Transform player;
	private Vector3 desiredPos;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		desiredPos.x = player.transform.position.x;
		desiredPos.z = player.transform.position.z;
		desiredPos.y = this.transform.position.y;
		this.transform.position = desiredPos;
	}
}
