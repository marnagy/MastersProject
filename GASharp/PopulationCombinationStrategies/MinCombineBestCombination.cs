public class MinCombineBestCombination<T> : PopulationCombinationStrategy<T> where T : Chromosome<T>
{
    public override T[] Combine(T[] oldPopulation, T[] newPopulation)
    {
        T[] combinedArr = new T[oldPopulation.Length + newPopulation.Length];

        Array.Copy(oldPopulation, 0, combinedArr, 0, oldPopulation.Length);
        Array.Copy(newPopulation, 0, combinedArr, oldPopulation.Length, newPopulation.Length);
        Array.Sort(combinedArr, (a,b) => a.Fitness < b.Fitness ? -1 : 1);

        return combinedArr[..oldPopulation.Length]
            .Select(ind => ind)
            .ToArray();
    }
}