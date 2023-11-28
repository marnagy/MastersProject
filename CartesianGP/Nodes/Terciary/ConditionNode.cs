public class ConditionNode : CartesianNode
{
    public ConditionNode(ParentIndices[] parents): base(parents) { }
    public override CartesianNode Clone()
    => new ConditionNode(this.Parents);

    public override CartesianNode Clone(ParentIndices[] newParents)
    => new ConditionNode(newParents);

    public override void Compute(CartesianChromosome chromosome)
    {
        var condNodeParents = Parents[0];
        var condResult = chromosome[condNodeParents.LayerIndex][condNodeParents.Index].Result;

        ParentIndices indices;
        if (condResult > 0)
            indices = Parents[1];
        else
            indices = Parents[2];
        
        this.Result = chromosome[indices.LayerIndex][indices.Index].Result;
    }

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
}