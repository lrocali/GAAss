using UnityEngine;
using System.Collections;

public class XWalk : MonoBehaviour {
	
	[SerializeField] float random;
	bool canCross = true;
	public GameObject [] roads;
	public GameObject [] blocks;
	Material green;
	Material red;
	public float high = -5.5f;
	public float low = -10f;
	public bool AllClear = true;
	public float RadiusSize;
	public LayerMask Enemy;
	public bool DrawGizmos;
	
	// Use this for initialization
	void Awake () {
		GenerateNew ();
		green = Resources.Load("road-xwalk-green", typeof(Material)) as Material;
		red = Resources.Load("road-xwalk-red", typeof(Material)) as Material;
		
	}
	void Start ()
	{
		RandomStartColour ();
	}

	void RandomStartColour ()
	{
		//more should be green to start
		if (random %3 == 0)
		{
			ChangeColour ();
		}

	}

	void UpdateBlocks ()
	{
		foreach (GameObject block in blocks)
		{
			
			
			Vector3 NewPos = block.transform.position;
			
			switch (canCross){
			case true:
				
				NewPos.y = low;
				
				
				break;
			case false:
				NewPos.y = high;
				
				break;
			default:
				break;
			}
			
			block.transform.position = NewPos;
		}
	}

	void UpdateRoads ()
	{
		foreach (GameObject road in roads)
		{
			
			switch (canCross){
			case true:
				road.GetComponent<MeshRenderer>().material = green;
				break;
			case false:
				road.GetComponent<MeshRenderer>().material = red;
				break;
			default:
				break;
			}
		}
	}

	void ChangeColour ()
	{
		AllClear = !Physics.CheckSphere (transform.position, RadiusSize, Enemy);
		//	Debug.Log ("AC: "+AllClear);
		if (!AllClear)
			return;
		
		canCross = !canCross;
		
		UpdateRoads ();
		UpdateBlocks();
		

		
		

		
		GameObject.Find("PathfindingManager").GetComponent<Grid>().CreateGrid();

	}
	
	// Update is called once per frame
	void Update () {
		random -= Time.deltaTime;
		
		if (random < 0)
		{
			ChangeColour ();
			GenerateNew ();
		}

	}
	
	void GenerateNew ()
	{
		random = (float) UnityEngine.Random.Range(10,20);
		
	}

	void OnDrawGizmos ()
	{
		if (!DrawGizmos)
			return;

		Gizmos.color = Color.green;
		Vector3 to = new Vector3 (0,0,RadiusSize);
		Gizmos.DrawLine(transform.position, transform.position-to );

	}
	
	
}
