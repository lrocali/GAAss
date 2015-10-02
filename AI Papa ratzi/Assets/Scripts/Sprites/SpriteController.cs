using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {

	Animator anim;
	public float scale;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		
	}
	

	public void FaceRight (bool isTrue) {


		if (isTrue)
			transform.localScale = new Vector3 (scale, scale, 1f);
		else
			transform.localScale = new Vector3 (-scale, scale, 1f);
		
	
	}


}
