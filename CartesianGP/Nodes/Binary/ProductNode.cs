public class ProductNode : BinaryNode
{
    public ProductNode(ParentIndices[] parents) : base(parents) { }
    public override CartesianNode Clone()
        => new ProductNode(this.Parents
            .Select(par => par)
            .ToArray()
        );

    public override CartesianNode Clone(ParentIndices[] newParents)
        => new ProductNode(newParents);

    public override void Compute(CartesianChromosome chromosome)
    {
        this.Result = Parents[..this.Arity]
            .Select(p => chromosome[p.LayerIndex][p.Index])
            .Select(node => node.Result)
            .Aggregate(1d, (a, b) => a * b);
    }

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
    // public override string ToString()
    // {
    //     return $"ProductNode:{this.Parents}";
    // }
}