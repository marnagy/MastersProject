public abstract class TertiaryNode: TreeNode
{
    protected TertiaryNode(TreeNode[]? children): base(children)
    {
        this.Arity = 3;
    }
}