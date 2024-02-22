using System.Text;

public class UnaryMinusNode : NodeFunctionality
{
    public UnaryMinusNode(): base(arity: 1) { }

    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return - children[0].Compute();
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append("-(");
        children[0].GetRepresentation(sb);
        sb.Append(')');
    }
}