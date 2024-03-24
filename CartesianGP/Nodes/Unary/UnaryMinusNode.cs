public class UnaryMinusNode : UnaryNode
{
    public UnaryMinusNode(ParentIndices[] parents): base(parents) { }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new UnaryMinusNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    => (-1d) * chromosome[this.Parents[0].LayerIndex][this.Parents[0].Index]
                .Compute(chromosome);

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is UnaryMinusNode otherUnaryMinusNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherUnaryMinusNode.Parents[parentIndex]);
        }

        return false;
    }
}