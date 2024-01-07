using System.Text;

public class SinNode : NodeFunctionality
{
    public SinNode(): base(arity: 1) { }

    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return Math.Sin(children[0].Compute());
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append("sin(");
        children[0].GetRepresentation(sb);
        sb.Append(')');
    }
}