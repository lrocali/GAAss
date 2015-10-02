using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float WalkSpeed;
	public float RunSpeed;
	Rigidbody rb;
	private bool RunPressed;
	private bool IsRunAvailable;

    public float PlayerLowSpeedLimit;

    //awareness
    PlayerAwareness pa;
	public float  AwarenessRateWhenRunning=10f;

    NN_GeneticAlgorithm ga;



	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		pa = GetComponent<PlayerAwareness>();

        ga = GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>();
	}
	
	// Update is called once per frame
	void Update () {

		RunPressed = (Input.GetKey(KeyCode.LeftShift) && pa.GetAwareness()<100);

		if (RunPressed)
		{
			//Debug.Log ("RUNPRESSED");
			pa.AddAwareness(AwarenessRateWhenRunning*Time.deltaTime);
		}

		float h = Input.GetAxisRaw("Horizontal") ;
		float v = Input.GetAxisRaw("Vertical") ;

		Vector3 moveDirection = new Vector3 (-h, 0, -v);

		// different velocity if Run is pressed
		rb.velocity = RunPressed ? 	moveDirection.normalized * RunSpeed :
									moveDirection.normalized * WalkSpeed;

        
	}

    public void NN_MovePlayer(float _h, float _v)
    {
        Vector3 moveDirection = new Vector3(_h, 0, _v);

        rb.velocity = moveDirection * WalkSpeed; //normalise?

        if (moveDirection.magnitude < PlayerLowSpeedLimit)
            PlayerMovingTooSlow();

    }

    void PlayerMovingTooSlow()
    {
        Debug.Log("Player too slow!");
        ga.PlayerTooSlow();
    }


    void OnCollisionStay(Collision other)
    {
        //Debug.Log("Col with " + other.transform.name);

       // if (other.transform.name != "Ground")
           // ga.CollisionDetected();
    }
}
