using UnityEngine;
using System.Collections;

public class PlayerSprite : MonoBehaviour {

	Animator anim;
	Rigidbody target;
	[SerializeField]
	float hSpeed;
	
	float vSpeed;
	public float scale;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		target = GetComponentInParent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		hSpeed = target.velocity.x;
		vSpeed = target.velocity.z;
		
		if (hSpeed < 0)
			transform.localScale = new Vector3 (-scale*2f, scale, 1f);
		else
			transform.localScale = new Vector3 (scale*2f, scale, 1f);
		
		anim.SetInteger("hSpeedInt", Mathf.RoundToInt(hSpeed)); 
	}
		

}
