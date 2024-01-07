using System.Text;

public class InputFunctionality: ValueFunctionality
{
    public readonly int InputIndex;
    public InputFunctionality(int inputIndex): base(value: 0)
    {
        this.InputIndex = inputIndex;
    }
    public override void GetRepresentation(StringBuilder sb, TreeNodeMaster[]? children)
    {
        sb.Append($"x_{this}");
    }
    public override string ToString()
    => $"InputNode(Index:{this.InputIndex})";
}