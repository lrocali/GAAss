using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NN_Layer
{
    NN_Neuron[] neurons;

    public NN_Layer(int neuronsCount, int inputCount, System.Random randomObject)
    {
        neurons = new NN_Neuron[neuronsCount];

        for (int i = 0; i < neuronsCount; i++)
        {
            neurons[i] = new NN_Neuron(inputCount, randomObject);
        }

    }

    public float[] Evaluate(float[] input)
    {
        //dynamicArray
        List<float> dynArray = new List<float> ();

        foreach (NN_Neuron neuron in neurons)
        {
            float activation = 0.0f;

            for (int i = 0; i < neuron.GetInputs().Length - 1; i++)
            {
                activation += input[i] * neuron.GetWeights()[i];
            }

            //add bias to act as threshold, fixed to -1
            activation += neuron.GetWeights()[neuron.GetWeights().Length - 1] * -1.0f;

            //calculate the sigmoid
            float sigmoid = 1 / (1 + Mathf.Exp(-activation));
            //Debug.Log("sigmoid value: " + sigmoid);

            //make it between -1 and 1 again
            sigmoid = sigmoid * 2 - 1;

            // push to outputs
            dynArray.Add(sigmoid);
        }

        float[] output = new float[dynArray.Count];

        for (int j = 0; j < dynArray.Count; j++)
        {
            output[j] = dynArray[j];
        }

        return output;
    }

    public NN_Neuron[] getNeurons()
    {
        return neurons;
    }
}
		
    
