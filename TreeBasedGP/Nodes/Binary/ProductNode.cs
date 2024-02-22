using System.Text;

public class ProductNode : NodeFunctionality
{
    public ProductNode(): base(arity: 2) { }
    public override double Compute(TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        return Enumerable.Range(0, this.Arity)
            .Select(childIndex => children[childIndex].Compute())
            .Aggregate(seed: 1d, (a, b) => a * b);
    }

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        ArgumentNullException.ThrowIfNull(children);

        sb.Append('(');
        children[0].GetRepresentation(sb);
        sb.Append(")*(");
        children[1].GetRepresentation(sb);
        sb.Append(')');
    }
}