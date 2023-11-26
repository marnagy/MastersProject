public class ProductNode : TreeNode
{
    public ProductNode(TreeNode?[] children): base(children) { }
    public override TreeNode Clone()
    {
        return new ProductNode(this.Children);
    }

    public override TreeNode Clone(TreeNode?[] children)
    => new ProductNode(children);

    public override void Compute()
    {
        foreach (var children in this.Children)
        {
            children?.Compute();
        }
        this.Result = this.Children[0].Result * this.Children[1].Result;
    }
}