public class SumNode : CartesianNode
{
    public SumNode(ParentIndices[] parents): base(parents) { }
    public override CartesianNode Clone()
        => new SumNode(this.Parents);

    public override CartesianNode Clone(ParentIndices[] newParents)
        => new SumNode(newParents);

    public override void Compute(CartesianChromosome chromosome)
    {
        this.Result = Parents
            .Select(p => chromosome[p.LayerIndex][p.Index])
            .Select(node => node.Result)
            .Aggregate(0d, (a,b) => a+b);
    }
}