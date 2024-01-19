using System.Security.Cryptography;

public class ChangeNodesMutation : Mutation<CombinedTreeChromosome>
{
    ChangeNodeMutation[] Mutations;
    public ChangeNodesMutation(double probability, int outputAmount,
            double percentageToChange,
            double terminalNodesProbability,
            IReadOnlyDictionary<NodeFunctionality, double> terminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> terminalNodes,
            IReadOnlyDictionary<NodeFunctionality, double> nonTerminalNodesProbabilities,
            IReadOnlyList<NodeFunctionality> nonTerminalNodes): base(probability)
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
    }
    public override CombinedTreeChromosome Mutate(CombinedTreeChromosome ind, int genNum)
    {
        Enumerable.Zip(this.Mutations, ind.Subchromosomes)
            .ForEach(tup => _ = tup.First.Mutate(tup.Second, genNum));
        return ind;
    }
}