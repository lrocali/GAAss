using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NN_Network {

    public enum NN_OUTPUT { Horizontal=0, Vertical, Count}; // h & v force to make up direction for player to move
    public NN_Inputs NNInputs;
    
    float[] inputs;
    NN_Layer inputLayer;
    NN_Layer[] hiddenLayers;
    NN_Layer outputLayer;
    float[] outputs;

    void Awake()
    {
        NNInputs = GameObject.Find("Player").GetComponent<NN_Inputs>();
        
    }

    public NN_Network(int neuronsPerHidden, System.Random randomObject)
    {
        NNInputs = GameObject.Find("Player").GetComponent<NN_Inputs>();
        int hiddenLayersCount = 1; // 1 hidden layer enough for now

       
        inputs = new float[NNInputs.numberOfInputs];
        hiddenLayers = new NN_Layer[hiddenLayersCount];

        for (int i = 0; i < hiddenLayersCount; i++)
        {
            hiddenLayers[i] = new NN_Layer(neuronsPerHidden, NNInputs.numberOfInputs, randomObject);
        }

        outputs = new float[(int) NN_OUTPUT.Count];

        outputLayer = new NN_Layer((int)NN_OUTPUT.Count, neuronsPerHidden, randomObject);
    }

    // called after new input is set by the GA script
    // will calculate the outputs
    public void UpdateNN()
    {
        
        outputs = new float[(int)NN_OUTPUT.Count]; // clear

        int i = 0;

        foreach (NN_Layer layer in hiddenLayers)
        {
            if (i > 0)
                inputs = outputs;

            outputs = layer.Evaluate(inputs); //evaluate middle layer
            i++;
        }

        inputs = outputs;

        outputs = outputLayer.Evaluate(inputs);

    }

    public void SetInputs ()
    {
        inputs = NNInputs.GetInputs();
    }

    public float[] GetOutputs()
    {
        return outputs;
    }

    public float[] GetTotalWeights()
    {
        List<float> totWeights = new List<float>();

        // weights from middle neurons
        foreach (var layer in hiddenLayers)
            foreach (var neuron in layer.getNeurons())
                foreach (var weight in neuron.GetWeights())
                    totWeights.Add(weight);

        // weights from output neurons
        foreach (var neuron in outputLayer.getNeurons())
            foreach (var weight in neuron.GetWeights())
                totWeights.Add(weight);

        //convert to float array
        float[] output = new float[totWeights.Count];
        for (int i = 0; i < totWeights.Count; i++)
            output[i] = totWeights[i];


        return output;
    }

    public void SetTotalWeights(float[] _weights)
    {
        

        int i = 0;
        foreach (var layer in hiddenLayers)
        {
            foreach (var neuron in layer.getNeurons())
            {
                for (int j = 0; j < neuron.GetWeights().Length; j++)
                {
                    neuron.SetWeight(i%neuron.GetWeights().Length, _weights[i]);
                    i++;
                }
            }
        }

        foreach (var neuron in outputLayer.getNeurons())
        {
            for (int k = 0; k < neuron.GetWeights().Length; k++)
            {
                neuron.SetWeight(i%neuron.GetWeights().Length, _weights[i]);
                i++;
            }
        }

       

    }


}
