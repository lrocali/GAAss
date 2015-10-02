using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NN_Inputs : MonoBehaviour {

    public int numberOfInputs=10; // make sure this is right!
    [SerializeField]
    float[] inputs;
    public float CheckSphereRadius;

    public float TotalHDistToTarget;
    public float TotalVDistToTarget;

    public Transform EndTarget;

    float LastChecked = 0f;
    Vector3 LastPosition;
    /*
    *   Input list:
       //old 0 - horizontal distance to next waypoint 
       //old 1 - vertical distance to next waypoint
       0- total h distance to target
       1- total v dist to target

        //to be added
        2-  distance to target
        3 - horizontal distance to closest hideout
        4 - vertical distance to closest hideout
        5 - awareness level
        6 - 
    */
    UnitPlayer unitPlayer;
    NN_GeneticAlgorithm ga;

    //raycasts
    // TL, TT, TR, RR, BR, BB, BL, LL
    Transform[] RayTransforms;
    RaycastHit[] RayHits;
    [Tooltip("If the ray doesnt hit anything what value should it have eg 100f")]
    public float noHitDistance;

    void Awake()
    {
        unitPlayer = GetComponent<UnitPlayer>();
        inputs = new float[numberOfInputs];
        ga = GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>();
        EndTarget = GameObject.Find("EndTrigger").transform;
        LastPosition = transform.position;


        //ray transforms
        RayTransforms = new Transform[4];
        //RayTransforms[0] = GameObject.Find("TL").transform;
        RayTransforms[0] = GameObject.Find("TT").transform;
        //RayTransforms[2] = GameObject.Find("TR").transform;
        RayTransforms[1] = GameObject.Find("RR").transform;
        //RayTransforms[4] = GameObject.Find("BR").transform;
        RayTransforms[2] = GameObject.Find("BB").transform;
        //RayTransforms[6] = GameObject.Find("BL").transform;
        RayTransforms[3] = GameObject.Find("LL").transform;

        RayHits = new RaycastHit[4];



    }

    void Update()
    {
        

        //inputs[0] = unitPlayer.HorizDistToNextWaypoint;
        //inputs[1] = unitPlayer.VertDistToNextWaypoint;
        inputs[0] = TotalHDistToTarget = EndTarget.position.z - transform.position.z;
        inputs[1] = TotalVDistToTarget = EndTarget.position.x - transform.position.x;

        CalculateRayHits();

        //passed to the neural network in the NN_Network script in their update function- SetInputs

        //check if stuck every 5 seconds
        LastChecked += Time.deltaTime;


        if (LastChecked > 1f)
        {
            if (Vector3.Distance(LastPosition, transform.position) < 1)
            {
                Debug.Log(gameObject.name + " was stuck! Kill it!");
                ga.CollisionDetected();
                

            }
            LastPosition = transform.position;
            LastChecked = 0f;
        }
		GameObject.Find ("InputText").GetComponent<Text> ().text = " " 
			+ "HD: " + inputs [0] + "\n"
				+ "VD: " + inputs [1] + "\n"
				+ "T: " + inputs [2] + "\n"
				+ "R: " + inputs [3] + "\n"
				+ "B: " + inputs [4] + "\n"
				+ "L: " + inputs [5] + "\n";
		/*Debug.Log ("HDist :"+inputs[0]);
		Debug.Log ("VDist :"+inputs[1]);
		Debug.Log ("TT :"+inputs[2]);
		Debug.Log ("RR :"+inputs[3]);
		Debug.Log ("BB :"+inputs[4]);
		Debug.Log ("LL :"+inputs[5]);*/
    }

    public float[] GetInputs()
    {
        return inputs;
    }

    public float GetSpecificInput(int _index)
    {
        return inputs[_index];
    }

    void CalculateRayHits()
    {
        int numRays = RayTransforms.Length;
        int numInputsbefore = 2; // 2 inputs before this count
        


        for (int i = 0; i < numRays; i++)
        {
            if (Physics.Raycast(RayTransforms[i].position, RayTransforms[i].forward, out RayHits[i]))
            {
                inputs[i + numInputsbefore] = RayHits[i].distance;
                
            }
            else
                inputs[i + numInputsbefore] = noHitDistance;

        }
    }
}
