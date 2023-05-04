abstract public class Fitness<T> where T: Chromosome<T>
{
    abstract public double ComputeFitness(T ind);
}