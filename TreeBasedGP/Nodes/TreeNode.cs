public abstract class TreeNode
{
    protected readonly TreeNode?[] Children;
    private const byte _childrenAmount = 3;
    public double Result { get; protected set; }
    protected TreeNode(TreeNode?[] children)
    {
        if (children.Length != TreeNode._childrenAmount)
            throw new Exception($"TreeNode requires exactly {TreeNode._childrenAmount} children");
        this.Children = children;
    }
    public abstract TreeNode Clone();
    public abstract TreeNode Clone(TreeNode?[] children);
    public abstract void Compute();
}