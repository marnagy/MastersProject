public class ProductNode : BinaryNode
{
    public ProductNode(TreeNode?[] children): base(children) { }
    public override TreeNode Clone()
    => new ProductNode(this.Children);

    public override TreeNode Clone(TreeNode?[] children)
    => new ProductNode(children);

    public override double Compute()
    => this.Children[0].Compute() * this.Children[1].Compute();
}