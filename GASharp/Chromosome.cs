public abstract class Chromosome<T> where T: Chromosome<T>
{
    public double Fitness { get; set; }
    public abstract T CreateNew();
    public abstract T Clone();
    public virtual void UpdateFitness(Fitness<T> fitness)
    {
        // ?: Can this be done without cast?
        this.Fitness = fitness.ComputeFitness((T)this);
        if (this.Fitness == 0d)
            this.Fitness = fitness.ComputeFitness((T)this);
    }
    public abstract bool IsValid();
}