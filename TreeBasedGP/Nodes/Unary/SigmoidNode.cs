public class SigmoidNode : UnaryNode
{
    public SigmoidNode(TreeNode[]? children): base(children)
    {
        ArgumentNullException.ThrowIfNull(children);
    }
    public override TreeNode Clone()
    => this.Clone(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new SinNode(children);

    public override double Compute()
    => 1 / (1 + Math.Exp(-this.Children[0].Compute()));

    public override string Representation()
    => $"sigmoid({this.Children[0].Representation()})";
}