public class ValueNode : CartesianNode
{
    public double Value { get; internal set; }
    public ValueNode(double value, ParentIndices[] parents) : base(parents)
    {
        this.Arity = 0;
        this.Value = value;
        this.Result = this.Value;
    }

    public override void Compute(CartesianChromosome chromosome) { }

    public override CartesianNode Clone()
        => new ValueNode(this.Value, CartesianNode.GetEmptyParents());

    public override CartesianNode Clone(ParentIndices[] newParents)
        => new ValueNode(this.Value, newParents);

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if ( other is ValueNode otherValueNode) {
            return this.Value == otherValueNode.Value
                && Enumerable.Range(0, this.Parents.Length)
                    .All(parentIndex => this.Parents[parentIndex] == otherValueNode.Parents[parentIndex]);
        }

        return false;
    }
}