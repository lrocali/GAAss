using UnityEngine;
using System.Collections;

public class NN_Neuron {

    float[] inputs;
    float[] weights;

    public NN_Neuron(int inputCount, System.Random randomObject)
    {
        inputs = new float[inputCount];
        weights = new float[inputCount + 1]; // extra for the Bias

        for (int i = 0; i < inputCount+1; i++)
        {
            weights[i] = (float) randomObject.NextDouble() *2 -1; //range between -1 and 1
           // Debug.Log("New Weight: " + weights[i]); // to check it is creating between -1 and 1
        }
    }

    public void SetInputs(float[] _inputs)
    {
        inputs = _inputs;
    }

    public float[] GetInputs()
    {
        return inputs;
    }

    public float[] GetWeights()
    {
        return weights;
    }

    public void SetWeights(float[] _weights)
    {
        weights = _weights;
    }

    public void SetWeight(int _weightPosition, float _weight)
    {
       // Debug.Log("weights.Length= " + weights.Length + ", _weightPosition: " + _weightPosition+ ", _weight: "+ _weight);
        weights[_weightPosition] = _weight;
    }


}
