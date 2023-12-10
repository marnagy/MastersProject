double terminalNodesProbability = 0.2d;
var inputNodes = Enumerable.Range(0, 2)
    .Select(i => new InputNode(1d + i, inputIndex: i))
    .ToArray();
var terminalNodesProbabilities = new Dictionary<TreeNode, double> {
    {new ValueNode(0d, null), 0.3d},
    {inputNodes[0], 0.3d},
    {inputNodes[1], 0.3d},
};
var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
    {new SumNode(
        children: [inputNodes[1], inputNodes[0], new ValueNode(3d, null)]
        ), 0.4d}
};
int? seed = null;


// how to handle setting inputs as terminals?
// for each input, update terminalNodesProbabilities dictionary (remove old inputs, add new ones)
TreeChromosome baseChromosome = new TreeChromosome(
    new SumNode(
        children: [inputNodes[1], inputNodes[0], new ValueNode(3d, null)]
        ),
    terminalNodesProbability,
    terminalNodesProbabilities,
    nonTerminalNodesProbabilities,
    seed
);

System.Console.WriteLine(baseChromosome);

inputNodes[0].Update(5d);
inputNodes[1].Update(20d);

System.Console.WriteLine(baseChromosome);
