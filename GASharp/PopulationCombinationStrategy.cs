public abstract class PopulationCombinationStrategy<T>
{
    public abstract T[] Combine(T[] oldPopulation, T[] newPopulation);
}