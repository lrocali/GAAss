using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[SerializeField]
	bool ShowMinimap =true;
	Camera minimap;

	// Use this for initialization
	void Start () {
		minimap = GameObject.Find("Minimap").GetComponent<Camera>();
		//Time.timeScale = 2.0F;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleMiniMap ()
	{
		ShowMinimap = ! ShowMinimap;

		if (ShowMinimap)
		{
			minimap.enabled=true;
		}
		else
		{
			minimap.enabled=false;
		}
	}

	public void QuitGame ()
	{
		Application.Quit();
	}

	public void ResetGame ()
	{
		Application.LoadLevel (0); 
	}
}
