public class DummyFitness : Fitness<CartesianChromosome>
{
    public override double ComputeFitness(CartesianChromosome ind)
        => ind.GetLayerSizes().Sum();
}