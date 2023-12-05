public class RemoveLayerMutation : Mutation<CartesianChromosome>
{
    public RemoveLayerMutation(double probability, int seed): base(probability, seed)
    {
        
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        throw new NotImplementedException();
    }
}