public class ElitismCombination<T> : PopulationCombinationStrategy<T> where T: Chromosome<T>
{
    private int BestAmount;
    private int NewIndividuals;
    private Fitness<T> Fitness;
    public ElitismCombination(int bestAmount, int newIndividuals, Fitness<T> fitnessFunc)
    {
        this.BestAmount = bestAmount;
        this.NewIndividuals = newIndividuals;
        this.Fitness = fitnessFunc;
    }
    public override T[] Combine(T[] oldPopulation, T[] newPopulation)
    {
        // sort
        Array.Sort(oldPopulation, (ind1, ind2) => ind1.Fitness < ind2.Fitness ? -1 : 1);

        T[] result = new T[oldPopulation.Length];

        // # Combine populations
        // elites
        Array.Copy(oldPopulation[..this.BestAmount], result, this.BestAmount);
        // new population
        Array.Copy(newPopulation, sourceIndex: 0,
            result, destinationIndex: this.BestAmount,
            newPopulation.Length - this.BestAmount - this.NewIndividuals);
        // create and add new individuals
        int newIndividualsStartIndex = newPopulation.Length - this.NewIndividuals;
        for (int i = 0; i < this.NewIndividuals; i++)
        {
            result[newIndividualsStartIndex + i] = newPopulation[0].CreateNew();
            result[newIndividualsStartIndex + i].UpdateFitness(this.Fitness);
        }

        return result;
    }
}