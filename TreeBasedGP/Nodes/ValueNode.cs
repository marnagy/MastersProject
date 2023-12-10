/// <summary>
/// Node representing a simple value.
/// It has <i>no Children</i> (ValueNode.Children is <b>null</b>)
/// </summary>
public class ValueNode : TreeNode
{
    private readonly double _value;
    public readonly string? Name;
    public ValueNode(double value, TreeNode[]? children, string? name = null): base(null)
    {
        this._value = value;
        this.Arity = 0;
        this.Name = name;
    }

    public override TreeNode Clone()
    => new ValueNode(this._value, this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new ValueNode(this._value, children);

    public override double Compute()
    => this._value;
    public override string ToString()
    => this.Name == null
        ? $"{this.GetType()}[{this._value}]"
        : $"{this.GetType()}[{this.Name}]";
}