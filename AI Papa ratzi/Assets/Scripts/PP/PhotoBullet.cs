using UnityEngine;
using System.Collections;

public class PhotoBullet : MonoBehaviour {

	PlayerAwareness pa;
	public int HitDamage=10;

	void Start ()
	{
		pa = GameObject.Find("Player").GetComponent<PlayerAwareness>();
		Destroy(this.gameObject, 2f);
	}

	void OnCollisionEnter(Collision collision) {
	
		if (collision.gameObject.name.StartsWith("BG"))
		{
			Destroy(this.gameObject);
//			Debug.Log("hit BG");
		}

		if (collision.gameObject.name.StartsWith("Player"))
		{
			Destroy(this.gameObject);
//			Debug.Log("hit player");
			pa.AddAwareness(HitDamage);
		}
			
	}
}
