using System.Text;

public class ProductNode : BinaryNode
{
    public ProductNode(ParentIndices[] parents) : base(parents) { }
    public override CartesianNode Clone(ParentIndices[] newParents)
        => new ProductNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    => Parents[..this.Arity]
        .Select(p => chromosome[p.LayerIndex][p.Index])
        .Select(node => node.Compute(chromosome))
        .Aggregate(1d, (a, b) => a * b);

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is ProductNode otherProductNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherProductNode.Parents[parentIndex]);
        }

        return false;
    }

    public override void GetRepresentation(StringBuilder sb, CartesianChromosome ind)
    {
        var firstParent = this.Parents[0];
        var secondParent = this.Parents[1];
        sb.Append('(');
        ind[firstParent.LayerIndex][firstParent.Index].GetRepresentation(sb, ind);
        sb.Append('*');
        ind[secondParent.LayerIndex][secondParent.Index].GetRepresentation(sb, ind);
        sb.Append(')');
    }
}