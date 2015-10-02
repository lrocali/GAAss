using UnityEngine;
using System.Collections;

public class PaparazziShoot : MonoBehaviour {

	Transform player;
	public GameObject bullet;
	public float bulletSpeed;
	Vector3 aim;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").transform;

	
	}
	
	public void ShootBullet ()
	{
		aim = player.position - this.transform.position;
//		Debug.Log ("AIM: "+ (Vector3) aim);
		GameObject clone;
		clone = (GameObject) Instantiate(bullet, this.transform.position + aim.normalized, Quaternion.identity);

		clone.GetComponent<Rigidbody>().velocity =  (aim *bulletSpeed);

	}
}
