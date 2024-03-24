public class ReLUNode : UnaryNode
{
    public ReLUNode(ParentIndices[] parents): base(parents)
    {
        
    }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new ReLUNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    {
        var parentValue = chromosome[this.Parents[0].LayerIndex][this.Parents[0].Index]
                .Compute(chromosome);
        if (parentValue < 0d)
            return 0d;
        else
            return parentValue;
    }
    
    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is ReLUNode otherReLUNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherReLUNode.Parents[parentIndex]);
        }

        return false;
    }
}