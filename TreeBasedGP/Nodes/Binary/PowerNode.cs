using System.Text;

public class PowerNode : NodeFunctionality
{
    public PowerNode(): base(arity: 2) { }
    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        var res1 = children[0].Compute();
        var res2 = children[1].Compute();

        return Math.Pow(res1, res2);
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append('(');
        children[0].GetRepresentation(sb);
        sb.Append(")^(");
        children[1].GetRepresentation(sb);
        sb.Append(')');
    }
}