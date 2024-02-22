using System.Text;

public class ReLUNode : NodeFunctionality
{
    public ReLUNode(): base(arity: 1) { }

    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        double childRes = children[0].Compute();

        if (childRes < 0)
            return 0d;
        else
            return childRes;
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append("ReLU(");
        children[0].GetRepresentation(sb);
        sb.Append(')');
    }
}