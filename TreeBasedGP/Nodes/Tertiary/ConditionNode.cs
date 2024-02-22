using System.Text;

public class ConditionNode: NodeFunctionality
{
    public ConditionNode(): base(arity: 3) { }
    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return children[0].Compute() > 0d ?
            children[1].Compute() :
            children[2].Compute();
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append("if (");
        children[0].GetRepresentation(sb);
        sb.Append(" > 0) then [");
        children[1].GetRepresentation(sb);
        sb.Append("] else [");
        children[2].GetRepresentation(sb);
        sb.Append(']');
    }
}