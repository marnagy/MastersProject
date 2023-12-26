public class SinNode : UnaryNode
{
    public SinNode(TreeNode[]? children): base(children)
    {
        ArgumentNullException.ThrowIfNull(children);
    }
    public override TreeNode Clone()
    => this.Clone(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new SinNode(children);

    public override double Compute()
    => Math.Sin(this.Children[0].Compute());

    public override string Representation()
    => $"sin({this.Children[0].Representation()})";
}