using UnityEngine;
using System.Collections;

public class PaparazziRecharge : MonoBehaviour {


	void OnTriggerEnter (Collider col)
	{
		Debug.Log("Entering pap building: " + col.gameObject.tag);

		if (col.gameObject.tag.Equals("PP"))
		{
			Debug.Log("hello pp");
			//col.gameObject.GetComponent<Unitpap>().ArrivedBackAtBase();
		}



	}


}
