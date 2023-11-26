public class ValueNode : TreeNode
{
    private readonly double _value;
    // public ValueNode(double value): base([null, null, null])
    // {
    //     this._value = value;
    // }
    public ValueNode(double value, TreeNode?[] children): base(children)
    {
        this._value = value;
    }

    public override TreeNode Clone()
    => new ValueNode(this._value, this.Children);

    public override TreeNode Clone(TreeNode?[] children)
    => new ValueNode(this._value, children);

    public override void Compute()
    {
        this.Result = _value;
    }
}