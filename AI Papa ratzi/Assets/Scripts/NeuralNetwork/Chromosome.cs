using UnityEngine;
using System.Collections;

public class Chromosome {

    int fitness;
    float[] weights;

    public Chromosome(int weightsCount, System.Random randomObject)
    {
        fitness = 0;
        weights = new float[weightsCount];

        for (int i = 0; i < weightsCount; i++)
        {
            weights[i] = (float) randomObject.NextDouble() * 2 - 1; // -1 to 1 range
        }
    }

    public Chromosome(float[] _weights)
    {
        fitness = 0;
        weights = _weights;
    }

    public void SetFitness(int _fitness)
    {
        fitness = _fitness;
    }

    public int GetFitness()
    {
        return fitness;
    }

    public void SetWeights(float[] _weights)
    {
        weights = _weights;
    }

    public float[] GetWeights()
    {
        
        return weights;
    }
}
