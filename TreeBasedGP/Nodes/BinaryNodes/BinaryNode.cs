using System.Security.Authentication;

public abstract class BinaryNode : TreeNode
{
    protected BinaryNode(TreeNode?[] children) : base(children)
    {
        this.Arity = 2;
    }
}