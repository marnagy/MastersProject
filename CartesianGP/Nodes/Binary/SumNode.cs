using System.Text;

public class SumNode : BinaryNode
{
    public SumNode(ParentIndices[] parents) : base(parents) { }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new SumNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    => Parents[..this.Arity]
        .Select(p => chromosome[p.LayerIndex][p.Index])
        .Select(node => node.Compute(chromosome))
        .Aggregate(0d, (a,b) => a+b);

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is SumNode otherSumNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherSumNode.Parents[parentIndex]);
        }

        return false;
    }
}