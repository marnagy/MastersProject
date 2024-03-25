using System.Text;

public class PowerNode : BinaryNode
{
    public PowerNode(ParentIndices[] parents) : base(parents) { }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new PowerNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    {
        var baseParent = this.Parents[0];
        var exponentParent = this.Parents[1];
        return Math.Pow(
            chromosome[baseParent.LayerIndex][baseParent.Index].Compute(chromosome),
            chromosome[exponentParent.LayerIndex][exponentParent.Index].Compute(chromosome)
        );
    }

    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is PowerNode otherPowerNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherPowerNode.Parents[parentIndex]);
        }

        return false;
    }

    public override void GetRepresentation(StringBuilder sb, CartesianChromosome ind)
    {
        var baseParent = this.Parents[0];
        var exponentParent = this.Parents[1];
        sb.Append('(');
        ind[baseParent.LayerIndex][baseParent.Index].GetRepresentation(sb, ind);
        sb.Append("**");
        ind[exponentParent.LayerIndex][exponentParent.Index].GetRepresentation(sb, ind);
        sb.Append(')');
    }
}