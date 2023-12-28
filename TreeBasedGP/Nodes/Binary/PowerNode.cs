public class PowerNode : BinaryNode
{
    public PowerNode(TreeNode[]? children): base(children) { }
    public override TreeNode Clone()
    => new PowerNode(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new PowerNode(children);

    public override double Compute()
    {
        if (this.Children is null)
            throw new ArgumentNullException($"Argument {nameof(this.Children)} cannot be null for node {this.GetType()}");
        return Math.Pow(this.Children[0].Compute(), this.Children[1].Compute());
    }

    public override string Representation()
    => $"(({this.Children[0].Representation()})**({this.Children[1].Representation()}))";
}