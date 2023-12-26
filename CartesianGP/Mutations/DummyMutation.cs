public class DummyMutation : Mutation<CartesianChromosome>
{
    public DummyMutation(double probability): base(probability) { }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
        => ind.Clone();
}