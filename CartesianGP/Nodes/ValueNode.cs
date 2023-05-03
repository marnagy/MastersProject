public class ValueNode : CartesianNode
{
    public double Value { get; internal set; }
    public ValueNode(double value, ParentIndices[] parents) : base(parents)
    {
        this.Value = value;
        this.Result = this.Value;
    }

    public override void Compute(CartesianChromosome chromosome) { }

    public override CartesianNode Clone()
        => new ValueNode(this.Value, CartesianNode.GetEmptyParents());

    public override CartesianNode Clone(ParentIndices[] newParents)
        => this.Clone();
}