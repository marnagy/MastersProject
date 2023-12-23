public class SumNode : BinaryNode
{
    public SumNode(TreeNode[]? children): base(children)
    {
        if (children is null)
            throw new ArgumentNullException($"Argument {nameof(children)} cannot be null for node {this.GetType()}");
    }
    public override TreeNode Clone()
    => this.Clone(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    {
        if (children is null)
            throw new ArgumentNullException($"Argument {nameof(children)} cannot be null for node {this.GetType()}");
        return new SumNode(
            children
                .Select(x => x.Clone())
                .ToArray()
        );
    }

    public override double Compute()
    {
        if (this.Children is null)
            throw new ArgumentNullException($"Argument {nameof(this.Children)} cannot be null for node {this.GetType()}");
        return Enumerable.Range(0, this.Arity)
            .Select(i => this.Children[i].Compute())
            .Aggregate(0d, (a, b) => a + b);
    }

    public override string Representation()
    => $"(({this.Children[0].Representation()}) + ({this.Children[1].Representation()}))";
}