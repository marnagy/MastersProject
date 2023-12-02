public abstract class TreeNode
{
    // TODO: change to IReadOnlyList
    internal readonly TreeNode[]? Children;
    public const int ChildrenAmount = 3;
    public bool HasChildren => this.Children is not null;
    public int Arity { get; protected init; }
    public double Result { get; protected set; }
    protected TreeNode(TreeNode[]? children)
    {
        if (children?.Length != TreeNode.ChildrenAmount)
            throw new Exception($"TreeNode requires exactly {TreeNode.ChildrenAmount} children");
        // if children is null, node does not have children
        this.Children = children;
    }
    public abstract TreeNode Clone();
    public abstract TreeNode Clone(TreeNode[]? children);
    public abstract double Compute();
}