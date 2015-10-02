using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Population {

    Chromosome[] chromosomes;
    int currentChromosome;//
    int currentPopulation;//

    Chromosome bestChromosome;
    int bestPopulation;

    [Tooltip("Should be a low number, around 0.008f chance of mutating the offspring")]
    public float MutationProbability = 0.008f;



   

    public Population(int _cCount, int _wCount, System.Random _randomObject)
    {
        chromosomes = new Chromosome[_cCount];

        for (int i = 0; i < _cCount; i++)
        {
            chromosomes[i] = new Chromosome(_wCount, _randomObject);

        }
        currentPopulation = 0;
        currentChromosome = 0;
        bestChromosome = chromosomes[0];
        bestPopulation = 0;

    }

    public void NewGeneration(System.Random _randomObject)
    {
        currentPopulation++;
        GameObject.Find("PopulationText").GetComponent<Text>().text = "Pop'ln #: "+ currentPopulation;

        ResetCurrentChromosome();
        Chromosome[] newChromosomes = new Chromosome[chromosomes.Length];
        float crossOverProb = 0.85f; // high probability

        for (int i = 0; i < chromosomes.Length; i = i + 2) //increment in pairs
        {
            // new mating chromosomes chosen with roulette wheel method
            Chromosome firstChrom = RouletteWheel(_randomObject); // should we force at least one to be the best chromosome
            Chromosome secondChrom = RouletteWheel(_randomObject);

            if (_randomObject.NextDouble() <= crossOverProb)
            {
                Chromosome[] chromosomePair = CrossOver(firstChrom, secondChrom, _randomObject);
                newChromosomes[i] = chromosomePair[0];
                newChromosomes[i+1] = chromosomePair[1];
                Debug.Log("Crossover");
            }

            else // use roulette
            {
                Debug.Log("Roulette option");
                newChromosomes[i] = firstChrom;
                newChromosomes[i + 1] = secondChrom;
            }

            //possible mutation
            Debug.Log("Sending1, weights[0]: " + newChromosomes[i].GetWeights()[0]);
            newChromosomes[i] = Mutate(ref newChromosomes[i], _randomObject);
            Debug.Log("Sending2, weights[0]: " + newChromosomes[i+1].GetWeights()[0]); //had error here
            newChromosomes[i + 1] = Mutate(ref newChromosomes[i + 1], _randomObject);
        }

        chromosomes = newChromosomes;
    }

    /*	"Roulette Wheel" is a method to get a chromosome based on its fitness value:
		higher fitness = higher chance of being picked
		- [Sum] Calculate sum of all chromosome fitnesses in population -> S.
	    - [Select] Generate random number from interval (0,S) -> r.
	    - [Loop] Go through the population and sum fitnesses from 0 to S -> s. 
	    When the sum s is greater than r, stop and return the chromosome where you are. */

    private Chromosome RouletteWheel(System.Random _randomObject)
    {
        int fitnessSum = 0;
        int randomNum;
        int selectedChrom = 0;

        foreach (Chromosome chrom in chromosomes)
        {
            fitnessSum += chrom.GetFitness();
        }
        Debug.Log("Fitness sum: " + fitnessSum);

        randomNum = _randomObject.Next(0, fitnessSum);
        fitnessSum = 0;

        foreach (Chromosome chrom2 in chromosomes)
        {
            fitnessSum += chrom2.GetFitness();
            if (fitnessSum > randomNum)
                break;
            else
                selectedChrom++;
        }

        Debug.Log("Roulette wheel has chosen chrom " + selectedChrom + " with weights[0]: " + chromosomes[selectedChrom].GetWeights()[0]);

        return chromosomes[selectedChrom];
    }

    /* "Crossover" will take two parent chromosomes as inputs, and produce two offspring
    chromosomes , a mix of their parents with some randomness */

    private Chromosome[] CrossOver(Chromosome _firstChrom, Chromosome _secondChrom, System.Random _RandomObj)
    {
        int totalWeights = _firstChrom.GetWeights().Length;
        int crossingPoint = _RandomObj.Next(0, totalWeights - 1); //where the chrom genes are split is randomised

        //crossover on weights
        float[] weights1 = new float[totalWeights]; //first offspring
        float[] weights2 = new float[totalWeights]; //second offspring

        for (int i = 0; i < totalWeights; i++)
        {
            if (i < crossingPoint)
            {
                weights1[i] = _firstChrom.GetWeights()[i];
                weights2[i] = _secondChrom.GetWeights()[i];
            }
            else
            {
                weights1[i] = _secondChrom.GetWeights()[i];
                weights2[i] = _firstChrom.GetWeights()[i];
            }
        }

        Chromosome[] chromPair = new Chromosome[2];
        chromPair[0] = new Chromosome(weights1);
        chromPair[1] = new Chromosome(weights2);

        return chromPair;
    }

    private Chromosome Mutate(ref Chromosome _chrom, System.Random _randomObject)
    {
        Debug.Log("Test, chrom weights[0]: " + _chrom.GetWeights()[0]);

        for (int i=0; i< _chrom.GetWeights().Length; i++)
        {
            if (_randomObject.NextDouble() <= MutationProbability)
            {
                Debug.Log("Weight "+i+" being mutated, old:" + _chrom.GetWeights()[i]);
                _chrom.GetWeights()[i] = (float) _randomObject.NextDouble() * 2 - 1; //still between -1 and 1 ** possibly should be += **
                Debug.Log("Weight " + i + " being mutated, new:" + _chrom.GetWeights()[i]);
            }
        }
        return _chrom;
    }

    //Getters and Setters

    public Chromosome[] GetChromosomes()
    {
        return chromosomes;
    }

    public Chromosome GetCurrentChromosome()
    {
        return chromosomes[currentChromosome];
    }

    public int GetCurrentChromosomeID()
    {
        return currentChromosome;
    }

    public bool IsLastChromosome()
    {
        return (currentChromosome == chromosomes.Length);
    }

    public void ResetCurrentChromosome()
    {
        currentChromosome = 0;
    }

    public void SetNextChromosome()
    {
        currentChromosome++;
    }

    public int GetCurrentChromosomeFitness()
    {
        return chromosomes[currentChromosome].GetFitness();
    }

    public void ResetCurrentChromosomeFitness()
    {
        chromosomes[currentChromosome].SetFitness(0);
    }

    public void SetCurrentChromosomeFitness(int _fit)
    {
        

        Text BestFitnessText = GameObject.Find("BestFitnessText").GetComponent<Text>();
        Text CurrentFitnessText = GameObject.Find("CurrentFitnessText").GetComponent<Text>();

        if (_fit < 0)
        {
          //  Debug.Log("Negative fitness capped to 0");
            return;
        }

        chromosomes[currentChromosome].SetFitness(_fit);
       // Debug.Log("Updating current fitness of "+ currentChromosome  + "to " + _fit + "Current best is: " + bestChromosome.GetFitness());
        CurrentFitnessText.text = ""+ _fit;

        
        if (int.Parse (CurrentFitnessText.text)> int.Parse(BestFitnessText.text))
        {
           // Debug.Log("ZZZZZZZZZ");
           // Debug.Log("Updating best fitness to "+_fit);
            bestPopulation = currentPopulation;
            bestChromosome = chromosomes[currentChromosome];
            BestFitnessText.text = "" + bestChromosome.GetFitness();
        }

      // Debug.Log("Updated current fitness to " + _fit + "Current best: " + bestChromosome.GetFitness());
        
    }

    public Chromosome GetBestChromosome()
    {
        return bestChromosome;
    }

    public void FitnessBonus(int _bonusAmt)
    {
        Debug.Log("BONUS! " + _bonusAmt);
        chromosomes[currentChromosome].SetFitness(chromosomes[currentChromosome].GetFitness() + _bonusAmt);
    }

}
