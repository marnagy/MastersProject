public class SinNode : UnaryNode
{
    public SinNode(ParentIndices[] parents): base(parents)
    {
        
    }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new SinNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    {
        var parentValue = chromosome[this.Parents[0].LayerIndex][this.Parents[0].Index]
                .Compute(chromosome);
        return Math.Sin(parentValue);
    }
    
    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is SinNode otherSinNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherSinNode.Parents[parentIndex]);
        }

        return false;
    }
}