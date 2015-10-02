using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NN_GeneticAlgorithm : MonoBehaviour {

    System.Random randomObject;
    [SerializeField]
    Population population;
    public bool isLearning;

    NN_Inputs NNInputs;
    NN_Network NNNetwork;

    float initialDistanceToTarget;
    public Transform EndGoalPosition;
    public Transform StartPosition;
    public Transform Player;

    PlayerController pc;
    [SerializeField]
    float DistanceTowardsGoal;
    [SerializeField]
    float ElapsedTime;

    float StartTimer;

    public float CheckMovingTimeToCheck;
    [Tooltip("Distance player has to have travelled towards the goal in CheckMovingTimeToCheck seconds")]
    public float CheckMovingDistance;

    public float TimeScale;
    [SerializeField]
    float BestTime=5000f;

    

    void Awake()
    {
        randomObject = new System.Random();
        Debug.Log(randomObject.NextDouble());
        
        NNNetwork = new NN_Network(64, randomObject);
        population = new Population(16, NNNetwork.GetTotalWeights().Length, randomObject);
        NNInputs = GameObject.Find("Player").GetComponent<NN_Inputs>();
        pc = Player.GetComponent<PlayerController>();
        initialDistanceToTarget = Vector3.Distance(StartPosition.position, EndGoalPosition.position);

        if (isLearning)
            Time.timeScale = TimeScale;
    }

    void Start()
    {
        StartSimulation();
    }

    void Update()
    {
        DistanceTowardsGoal = GetDistanceTowardsGoal();

        ElapsedTime = Time.time - StartTimer;
    }

    void FixedUpdate()
    {
        //get the inputs
        NNNetwork.SetInputs();

        // process inputs
        NNNetwork.UpdateNN();

        //get the outputs
        float[] outputs = NNNetwork.GetOutputs();

        //process output
        pc.NN_MovePlayer(outputs[0], outputs[1]);
        //pc.NN_MovePlayer(NNInputs.GetSpecificInput(0), NNInputs.GetSpecificInput(1)); //for testing

        //update fitness
        if (isLearning)
        {
            // update the fitness value
            UpdateFitness();
        }
    }

    void StartSimulation()
    {
        Debug.Log("Simulation starting");

        //cancel checkmoving
       // CancelInvoke("CheckMoving");

        
        //reset any movement to 0
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //update text
        if (GameObject.Find("EndGameText").GetComponent<Text>().text != "")
            GameObject.Find("EndGameText").GetComponent<Text>().text = "";

        GameObject.Find("ChromText").GetComponent<Text>().text = "Chrom#: 0" + population.GetCurrentChromosomeID();


        //start timer
        StartTimer = Time.time;

        if (isLearning)
        {
            //set Neural Network
            NNNetwork.SetTotalWeights(population.GetCurrentChromosome().GetWeights());
        }

        //reset fitness values

        // check if player is making decent progress after x seconds
        //Invoke("CheckMoving", CheckMovingTimeToCheck);
    }

    public void SetNormalTimeScale()
    {
        Time.timeScale = 1f;
    }

    void CheckMoving()
    {
        Debug.Log("Checking for movement");

        if (DistanceTowardsGoal < CheckMovingDistance)
        {
            Debug.Log("too slow! restart");
            RestartSimulation();
        }
    }

    public void CollisionDetected()
    {
        Debug.Log("CollisionDetected");
        RestartSimulation();
    }

    public void PlayerTooSlow()
    {
        Debug.Log("TooSlow");
        RestartSimulation();
    }

    public void FitnessBonus(int _bonusAmt)
    {
       // Debug.Log("BONUS! " + _bonusAmt);
        population.FitnessBonus(_bonusAmt);
    }


    void UpdateFitness()
    {
        
        
        int currentFitness = Mathf.RoundToInt(DistanceTowardsGoal);

        

        if (currentFitness < 0)
            return;

        population.SetCurrentChromosomeFitness(currentFitness);
    }

    public float GetDistanceTowardsGoal()
    {
        float distanceCovered = Vector3.Distance(Player.position, EndGoalPosition.position);
        return initialDistanceToTarget - distanceCovered;
    }

    void RestartSimulation()
    {
        if (isLearning)
        {
            Debug.Log("Current chromosome: " + population.GetCurrentChromosomeID()
            + " with fitness " + population.GetCurrentChromosomeFitness());

            //next chromo
            population.SetNextChromosome();

            if (population.IsLastChromosome())
            {
                // start new generation
                Debug.Log("Generation tested, start new one");
                population.NewGeneration(randomObject);
            }

            StartTimer = Time.time;
        }

        //put player back to start position
        Player.position = StartPosition.position;
		Player.GetComponent<PlayerAwareness>().ResetAwareness();

        //paparatzi's too
        GameObject[] PP = GameObject.FindGameObjectsWithTag("PP");
        foreach (GameObject P in PP)
        {
            P.GetComponent<Unitpap>().ResetPositions();
        }

        //and school girls
        GameObject[] SG = GameObject.FindGameObjectsWithTag("SG");
        foreach (GameObject S in SG)
        {
            if (S.name.StartsWith("school-girl-leader"))
            {
                Debug.Log(S.name);
                S.GetComponent<UnitSG>().ResetPositions();
            }
            else
                S.GetComponent<FollowSG>().ResetPositions();
        }

        //and BG;s
        GameObject[] BG = GameObject.FindGameObjectsWithTag("BG");
        foreach (GameObject B in BG)
        {
            B.GetComponent<BodyGuardFollow>().ResetPositions();
        }


        StartSimulation();


    }

    public float GetElapsedTime()
    {
        return ElapsedTime;
    }

    public void UpdateBestTime(float _bestTime)
    {
        if (_bestTime < BestTime)
        {
            BestTime = _bestTime;
        }
    }

}
