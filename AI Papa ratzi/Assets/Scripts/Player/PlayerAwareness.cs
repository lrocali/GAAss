using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerAwareness : MonoBehaviour {

	Slider slider;
	[SerializeField] float Awareness;
	public bool DrawGizmos;

	// Use this for initialization
	void Start () {
		slider = GameObject.Find("AwarenessSlider").GetComponent<Slider>();
	}
	
	public void AddAwareness (float addition)
	{
		Awareness += addition;
		if (Awareness >100)
			Awareness = 100;
		slider.value = Awareness;

	}

	public void LoseAwareness (float deduction)
	{
		Awareness -= deduction;
		if (Awareness < 0)
			Awareness=0;
		slider.value = Awareness;

	}

	public float GetAwareness ()
	{
		return Awareness;
	}

    public void ResetAwareness()
    {
        Awareness = 0;
        slider.value = Awareness;
    }




	void OnDrawGizmos() {
		if (DrawGizmos)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position, (float) Awareness);

		}
	}
}
