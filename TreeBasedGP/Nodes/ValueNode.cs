public class ValueNode : TreeNode
{
    private readonly double _value;
    // public ValueNode(double value): base([null, null, null])
    // {
    //     this._value = value;
    // }
    public ValueNode(double value, TreeNode[]? children): base(children)
    {
        this._value = value;
        this.Arity = 0;
    }

    public override TreeNode Clone()
    => new ValueNode(this._value, this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new ValueNode(this._value, children);

    public override double Compute()
    => this._value;
}