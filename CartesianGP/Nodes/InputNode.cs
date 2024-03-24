public class InputNode : ValueNode
{
    public readonly int InputIndex;
    public InputNode(double value, int inputIndex, ParentIndices[] parents) : base(value, parents)
    {
        this.InputIndex = inputIndex;
    }

    public override double Compute(CartesianChromosome chromosome)
    => this.Value;
    internal void SetValue(double value)
    {
        this.Value = value;
    }

    public override CartesianNode Clone(ParentIndices[] newParents)
        => new InputNode(this.Value, this.InputIndex, newParents);

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is InputNode otherInputNode)
        {
            return this.Value == otherInputNode.Value
                && Enumerable.Range(0, this.Parents.Length)
                    .All(parentIndex => this.Parents[parentIndex] == otherInputNode.Parents[parentIndex]);
        }

        return false;
    }
    public override string ToString()
    {
        return $"InputNode:x_{this.InputIndex}";
    }
}