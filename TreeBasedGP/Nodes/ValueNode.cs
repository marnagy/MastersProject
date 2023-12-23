/// <summary>
/// Node representing a simple value.
/// It has <i>no Children</i> (ValueNode.Children is <b>null</b>)
/// </summary>
public class ValueNode : TreeNode
{
    public double _value { get; protected set; }
    public ValueNode(double value): base(null)
    {
        this._value = value;
        this.Arity = 0;
    }
    public ValueNode(double value, TreeNode[]? _children): this(value) { }

    public override TreeNode Clone()
    => new ValueNode(this._value, this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new ValueNode(this._value, children);

    public override double Compute()
    => this._value;
    public override string ToString()
    => $"{this.GetType()}[{this._value}]";

    public override string Representation()
    => this._value.ToString();
}