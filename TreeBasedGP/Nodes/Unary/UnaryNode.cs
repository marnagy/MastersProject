public abstract class UnaryNode: TreeNode
{
    protected UnaryNode(TreeNode[]? children): base(children)
    {
        this.Arity = 1;
    }
}