using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndTrigger : MonoBehaviour {
	
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag =="Player")
		{


            GameObject.Find("EndGameText").GetComponent<Text>().text = "Game Over! Time: " + GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>().GetElapsedTime();
            GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>().UpdateBestTime(GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>().GetElapsedTime());
            GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>().FitnessBonus(1000);
            GameObject.Find("GameManager").GetComponent<NN_GeneticAlgorithm>().CollisionDetected();//kill it
        }
		
	}
	
}