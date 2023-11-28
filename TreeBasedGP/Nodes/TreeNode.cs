public abstract class TreeNode
{
    internal readonly TreeNode?[] Children;
    public const int ChildrenAmount = 3;
    public int Arity { get; protected init; }
    public double Result { get; protected set; }
    protected TreeNode(TreeNode?[] children)
    {
        if (children.Length != TreeNode.ChildrenAmount)
            throw new Exception($"TreeNode requires exactly {TreeNode.ChildrenAmount} children");
        this.Children = children;
    }
    public abstract TreeNode Clone();
    public abstract TreeNode Clone(TreeNode?[] children);
    public abstract double Compute();
}