public class DummyFitness : Fitness<CartesianChromosome>
{
    public override double ComputeFitness(CartesianChromosome ind)
        => ind.GetLayerSizes().Sum();

    public override void ComputeFitnessPopulation(CartesianChromosome[] population)
    {
        foreach (var ind in population)
        {
            this.ComputeFitness(ind);
        }
    }
}