public abstract class Chromosome<T>
{
    public double Fitness { get; set; }
    public abstract T CreateNew();
    public abstract Chromosome<T> Clone();
    public virtual void UpdateFitness(Fitness<Chromosome<T>> fitness)
    {
        this.Fitness = fitness.ComputeFitness(this);
    }
}