public class ProductNode : BinaryNode
{
    public ProductNode(TreeNode[]? children): base(children) { }
    public override TreeNode Clone()
    => new ProductNode(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new ProductNode(children);

    public override double Compute()
    {
        if (this.Children is null)
            throw new ArgumentNullException($"Argument {nameof(this.Children)} cannot be null for node {this.GetType()}");
        return Enumerable.Range(0, this.Arity)
            .Select(i => this.Children[i].Compute())
            .Aggregate(0d, (a, b) => a * b);
    }

    public override string Representation()
    => $"(({this.Children[0].Representation()})*({this.Children[1].Representation()}))";
}