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
        T[] masterArr = new T[oldPopulation.Length + newPopulation.Length];
        for (int i = 0; i < oldPopulation.Length; i++)
        {
            masterArr[i] = oldPopulation[i];
        }
        for (int i = 0; i < newPopulation.Length; i++)
        {
            masterArr[oldPopulation.Length + i] = newPopulation[i];
        }

        // sort
        Array.Sort(masterArr, (ind1, ind2) => ind1.Fitness < ind2.Fitness ? -1 : 1);

        T[] result = new T[oldPopulation.Length];

        // add new individuals
        Array.Copy(masterArr[..this.BestAmount], result, this.BestAmount);
        Array.Copy(newPopulation, sourceIndex: 0,
            result, destinationIndex: this.BestAmount,
            newPopulation.Length - this.BestAmount - this.NewIndividuals);
        int newIndividualsStartIndex = newPopulation.Length - this.NewIndividuals;
        for (int i = 0; i < this.NewIndividuals; i++)
        {
            result[newIndividualsStartIndex + i] = newPopulation[0].CreateNew();
            result[newIndividualsStartIndex + i].UpdateFitness(this.Fitness);
        }

        return result;
    }
}