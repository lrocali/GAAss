using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {

    [Tooltip("Distance the player should get to the next waypoint before triggering the next waypoint as the next target node")]
    public float waypointDistance;

    [Tooltip("Distance the player gets away from current waypoint before it is recalculated ")]
    public float pathRecalcDistance;

    public float HorizDistToNextWaypoint;
    public float VertDistToNextWaypoint;

    NN_GeneticAlgorithm ga;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        targetIndex = 0;
        ga = GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>();
    }

    // Update is called once per frame
    void Update () {

        //game hasnt started or has finished
        if (path.Length == 0)
            return;

        if (Vector3.Distance (transform.position, path[targetIndex]) < waypointDistance)
        {

            //increment index
            if (targetIndex < path.Length-1)
                targetIndex++;

            //bonus
           // ga.FitnessBonus(10);
        }

        if (Vector3.Distance(transform.position, path[targetIndex]) > pathRecalcDistance)
        {
            targetIndex = 0;
            RecalculatePath();
        }

        UpdateDistances();

    }

    void UpdateDistances()
    {
        
        //players current
        float currentX = transform.position.x;
        float currentZ = transform.position.z;

        //goal target pos
        float targetX = path[targetIndex].x;
        float targetZ = path[targetIndex].z;

        float actualDistance = Vector3.Distance(transform.position, path[targetIndex]);

        HorizDistToNextWaypoint = (targetX - currentX) / actualDistance; //normalize
        VertDistToNextWaypoint = (targetZ - currentZ) / actualDistance;  //normalize
    }
}
