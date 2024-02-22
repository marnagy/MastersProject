using System.Text;

public class ValueFunctionality : NodeFunctionality
{
    public double Value;
    public ValueFunctionality(double value): base(arity: 0)
    {
        this.Value = value;
    }
    public override double Compute(TreeNodeMaster[]? children)
    => this.Value;

    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        sb.Append(this.Value);
    }
    public override string ToString()
    => $"ValueNode({this.Value})";
}