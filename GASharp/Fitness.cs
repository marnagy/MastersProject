abstract public class Fitness<T> where T: Chromosome<T>
{
    public abstract double ComputeFitness(T ind);
    /// <summary>
    /// Thread-safe implementation of computing fitness for whole population.
    /// </summary>
    public virtual void ComputeFitnessPopulation(T[] population)
    {
        foreach (var ind in population)
        {
            this.ComputeFitness(ind);
        }
    }
}