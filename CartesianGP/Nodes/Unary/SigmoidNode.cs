public class SigmoidNode : UnaryNode
{
    public SigmoidNode(ParentIndices[] parents): base(parents)
    {
        
    }
    public override CartesianNode Clone(ParentIndices[] newParents)
    => new SigmoidNode(newParents);

    public override double Compute(CartesianChromosome chromosome)
    => this.ComputeSigmoid(
        chromosome[this.Parents[0].LayerIndex][this.Parents[0].Index]
                .Compute(chromosome)
    );
    
    private double ComputeSigmoid(double value)
    => 1d / (1d + Math.Exp(-value));
    
    public override bool Equals(CartesianNode? other)
    {
        if (other is null)
            return false;

        if (other is SigmoidNode otherSigmoidNode)
        {
            return Enumerable.Range(0, this.Parents.Length)
                .All(parentIndex => this.Parents[parentIndex] == otherSigmoidNode.Parents[parentIndex]);
        }

        return false;
    }
}