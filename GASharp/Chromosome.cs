public abstract class Chromosome<T> where T: Chromosome<T>
{
    public double Fitness { get; set; }
    public abstract T CreateNew();
    public abstract T Clone();
    public virtual void UpdateFitness(Fitness<T> fitness)
    {
        this.Fitness = fitness.ComputeFitness((T)this);
    }
    public abstract bool IsValid();
}