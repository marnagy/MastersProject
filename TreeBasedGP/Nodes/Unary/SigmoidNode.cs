using System.Text;

public class SigmoidNode : NodeFunctionality
{
    public SigmoidNode(): base(arity: 1) { }

    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return 1 / (1 + Math.Exp(-children[0].Compute()));
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append("sigmoid(");
        children[0].GetRepresentation(sb);
        sb.Append(')');
    }
}