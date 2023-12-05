public class DummyMutation : Mutation<CartesianChromosome>
{
    public DummyMutation(double probability, int? seed = null): base(probability, seed)
    {
        
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
        => ind.Clone();
}