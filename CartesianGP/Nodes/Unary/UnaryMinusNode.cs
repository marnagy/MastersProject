public class UnaryMinusNode : UnaryNode
{
    public UnaryMinusNode(ParentIndices[] parents): base(parents)
    {
        
    }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new UnaryMinusNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(CartesianNode? other)
    {
        throw new NotImplementedException();
    }
}