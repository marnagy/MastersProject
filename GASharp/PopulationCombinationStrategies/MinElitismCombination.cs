public class MinElitismCombination<T> : PopulationCombinationStrategy<T> where T: Chromosome<T>
{
    private int BestAmount;
    private int NewIndividuals;
    private Fitness<T> Fitness;
    private Func<T> CreateNewChromosome;
    public MinElitismCombination(int bestAmount, int newIndividuals, Fitness<T> fitnessFunc, Func<T> createNewChrom)
    {
        this.BestAmount = bestAmount;
        this.NewIndividuals = newIndividuals;
        this.Fitness = fitnessFunc;
        this.CreateNewChromosome = createNewChrom;
    }
    public override T[] Combine(T[] oldPopulation, T[] newPopulation)
    {
        // sort
        Array.Sort(oldPopulation, (ind1, ind2) => ind1.Fitness < ind2.Fitness ? -1 : 1);

        T[] result = new T[oldPopulation.Length];

        // # Combine populations
        // elites
        Array.Copy(oldPopulation, 0, result, 0, this.BestAmount);
        // new population
        Array.Copy(newPopulation, sourceIndex: 0,
            result, destinationIndex: this.BestAmount,
            newPopulation.Length - this.BestAmount - this.NewIndividuals);

        // create and add new individuals
        int newIndividualsStartIndex = newPopulation.Length - this.NewIndividuals;
        for (int i = 0; i < this.NewIndividuals; i++)
        {
            result[newIndividualsStartIndex + i] = this.CreateNewChromosome();
            result[newIndividualsStartIndex + i].UpdateFitness(this.Fitness);
        }

        return result;
    }
}