using System.Text;
using System.Threading.Channels;

public class SumNode : NodeFunctionality
{
    public SumNode(): base(arity: 2) { }
    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return Enumerable.Range(0, this.Arity)
            .Select(childIndex => children[childIndex].Compute())
            .Aggregate(seed: 0d, (a, b) => a + b);
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append('(');
        children[0].GetRepresentation(sb);
        sb.Append(")+(");
        children[1].GetRepresentation(sb);
        sb.Append(')');
    }
}