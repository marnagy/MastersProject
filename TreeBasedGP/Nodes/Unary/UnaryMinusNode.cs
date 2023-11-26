public class UnaryMinusNode : TreeNode
{
    public UnaryMinusNode(TreeNode?[] children): base(children) { }
    public override TreeNode Clone()
    => new UnaryMinusNode(this.Children);

    public override TreeNode Clone(TreeNode?[] children)
    => new UnaryMinusNode(children);

    public override void Compute()
    {
        this.Children[0].Compute();
        this.Result = -this.Children[0].Result;
    }
}