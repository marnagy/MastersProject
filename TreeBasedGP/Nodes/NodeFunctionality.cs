using System.Text;

public abstract class NodeFunctionality
{
    public readonly int Arity;
    public bool NeedsChildren => this.Arity > 0;
    protected NodeFunctionality(int arity)
    {
        this.Arity = arity;
    }
    public abstract double Compute(TreeNodeMaster[]? children);
    public abstract void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children);
}