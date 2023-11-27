using System.Runtime.InteropServices;

public class ChangeNodeMutation : Mutation<TreeChromosome>
{
    private readonly Dictionary<TreeNode, double> _nodeCatalogue;
    private static readonly Random _rng = new();
    public ChangeNodeMutation(double probability, Dictionary<TreeNode, double> nodeCatalogue): base(probability)
    {
        this._nodeCatalogue = nodeCatalogue;
    }
    public override TreeChromosome Mutate(TreeChromosome ind, int genNum)
    {
        int nodeAmount = Enumerable.Range(0, ind.Depth)
            .Select(level => TreeNode.ChildrenAmount.Pow(level))
            .Sum();
        
        var shouldChangeNode = Enumerable.Range(0, nodeAmount)
            .Select(_ => _rng.NextDouble())
            .Select(prob => prob < this.MutationProbability)
            .ToArray();
        
        // TODO: continue here
    }
    private int IntegerPow(int baseNum, int power)
    {
        if (power < 0)
            throw new ArgumentOutOfRangeException($"Argument {nameof(power)} needs to be >= 0.");
        if (baseNum <= 0)
            throw new ArgumentOutOfRangeException($"Argument {nameof(baseNum)} needs to be > 0.");

        int res = 1;
        for (int i = 1; i <= power; i++)
        {
            res = res * baseNum;
        }
        return res;
    }
}