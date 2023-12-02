public class SumNode : BinaryNode
{
    public SumNode(TreeNode?[] children): base(children) { }
    public override TreeNode Clone()
    {
        if (this.Children[0] is null)
            throw new NullReferenceException("Child 0 is null");
        if (this.Children[1] is null)
            throw new NullReferenceException("Child 1 is null");
        return new SumNode(
            this.Children
                .Select(x => x?.Clone())
                .ToArray()
        );
    }

    public override TreeNode Clone(TreeNode?[] children)
    {
        throw new NotImplementedException();
    }

    public override double Compute()
    => this.Children[0].Compute() + this.Children[1].Compute();
}