public class UnaryMinusNode : TreeNode
{
    public UnaryMinusNode(TreeNode[]? children): base(children)
    {
        if (children is null)
            throw new ArgumentNullException($"Argument {nameof(children)} cannot be null for node {this.GetType()}");
    }
    public override TreeNode Clone()
    => new UnaryMinusNode(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new UnaryMinusNode(children);

    public override double Compute()
    {
        if (this.Children is null)
            throw new ArgumentNullException($"Argument {nameof(this.Children)} cannot be null for node {this.GetType()}");
        return - this.Children[0].Compute();
    }
}