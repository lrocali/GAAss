using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowPPressed : MonoBehaviour {
	Image i ;
	public Color green;
	public Color white;

	// Use this for initialization
	void Start () {
		i= GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKey(KeyCode.P))
			i.color = green;
		else
			i.color = white;
	}
}
