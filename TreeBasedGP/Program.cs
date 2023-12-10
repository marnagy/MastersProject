double terminalNodesProbability = 0.2d;
var terminalNodesProbabilities = new Dictionary<TreeNode, double> {
    {new ValueNode(0d, null), 0.3d}
};
var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
    {new SumNode(
        children: [new ValueNode(1d, null), new ValueNode(2d, null), new ValueNode(3d, null)]
        ), 0.4d}
};
int? seed = null;

TreeChromosome baseChromosome = new TreeChromosome(
    new SumNode(
        children: [new ValueNode(1d, null), new ValueNode(2d, null), new ValueNode(3d, null)]
        ),
    terminalNodesProbability,
    terminalNodesProbabilities,
    terminalNodesProbabilities.Keys.ToArray(),
    nonTerminalNodesProbabilities,
    nonTerminalNodesProbabilities.Keys.ToArray(),
    seed
);

System.Console.WriteLine(baseChromosome);
