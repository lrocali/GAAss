using UnityEngine;
using System.Collections;

public class AlwaysFaceUp : MonoBehaviour {

	public Transform BG;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//transform.localRotation = Quaternion.Euler(-transform.parent.rotation.eulerAngles + trueRotation);
		Vector3 Newpos = BG.position;
		Newpos.y = 0; 

		transform.position = Newpos;
	}
}
