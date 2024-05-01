using System.Text;

public class ConditionNode : CartesianNode
{
    public ConditionNode(ParentIndices[] parents): base(parents) { }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new ConditionNode(newParents);
    private CartesianNode GetConditionNode(CartesianChromosome chromosome)
    => chromosome[this.Parents[0].LayerIndex][this.Parents[0].Index];
    private CartesianNode GetTrueNode(CartesianChromosome chromosome)
    => chromosome[this.Parents[1].LayerIndex][this.Parents[1].Index];
    private CartesianNode GetFalseNode(CartesianChromosome chromosome)
    => chromosome[this.Parents[2].LayerIndex][this.Parents[2].Index];

    public override double Compute(CartesianChromosome chromosome)
    => this.GetConditionNode(chromosome).Compute(chromosome) > 0
        ? this.GetTrueNode(chromosome).Compute(chromosome)
        : this.GetFalseNode(chromosome).Compute(chromosome);

    public override bool Equals(CartesianNode? other)
    {
        if (other is ConditionNode otherCondition)
        {
            return Enumerable.Zip(this.Parents, otherCondition.Parents)
                .All(indices => indices.First == indices.Second);
        }
        else
            return false;
    }

    public override void GetRepresentation(StringBuilder sb, CartesianChromosome ind)
    {
        sb.Append("[if ");
        this.GetConditionNode(ind).GetRepresentation(sb, ind);
        sb.Append(" > 0) then (");
        this.GetTrueNode(ind).GetRepresentation(sb, ind);
        sb.Append(") else (");
        this.GetFalseNode(ind).GetRepresentation(sb, ind);
        sb.Append(")]");
    }
}