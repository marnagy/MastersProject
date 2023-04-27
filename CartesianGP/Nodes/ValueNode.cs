public class ValueNode : CartesianNode
{
    public double Value { get; internal set; }
    public ValueNode(double value) : base(CartesianNode.EmptyParents)
    {
        this.Value = value;
        this.Result = this.Value;
    }

    public override void Compute() { }

    public override CartesianNode Clone()
        => new ValueNode(this.Value);
}