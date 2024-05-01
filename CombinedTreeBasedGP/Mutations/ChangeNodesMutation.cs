using System.Security.Cryptography;

public class ChangeNodesMutation : Mutation<CombinedTreeChromosome>
{
    ChangeNodeMutation[] Mutations;
    private readonly int MaxThreads;
    public ChangeNodesMutation(double probability, int outputAmount,
            double percentageToChange,
            double terminalNodesProbability,
            IReadOnlyDictionary<NodeFunctionality, double> terminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> terminalNodes,
            IReadOnlyDictionary<NodeFunctionality, double> nonTerminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> nonTerminalNodes,
            int maxThreads): base(probability)
    {
        this.Mutations = Enumerable.Range(0, outputAmount)
            .Select(_ => new ChangeNodeMutation(
                probability,
                percentageToChange,
                terminalNodesProbability,
                terminalNodesProbabilities,
                terminalNodes,
                nonTerminalNodesProbabilities,
                nonTerminalNodes
            ))
            .ToArray();
        this.MaxThreads = maxThreads;
    }
    public override CombinedTreeChromosome Mutate(CombinedTreeChromosome ind, int genNum)
    {
        Enumerable.Zip(this.Mutations, ind.Subchromosomes)
            .ForEach(tup => _ = tup.First.Mutate(tup.Second, genNum));
        return ind;
    }
}