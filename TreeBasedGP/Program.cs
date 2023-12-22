using System.Globalization;
using CommandLine;

class Program
{
    public static void Main(string[] args)
    {
        // set to en-us culture -> interpret real number with decimal point instead of decimal comma
        // from https://stackoverflow.com/questions/2234492/is-it-possible-to-set-the-cultureinfo-for-an-net-application-or-just-a-thread#comment32681459_2247570
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        Options cliArgs = Parser.Default.ParseArguments<Options>(args).Value;
        if (cliArgs == null)  // --help case
            return;

        if (!CheckArgs(cliArgs))
        {
            System.Console.Error.WriteLine("Invalid arguments.");
            System.Console.Error.WriteLine(cliArgs);
        }

        double terminalNodesProbability = cliArgs.TerminalNodesProbability;

        // prepare CSV
        (double[,] inputs, double[,] outputs) = CSVHelper.PrepareCSV(
            cliArgs.CSVFilePath,
            cliArgs.CSVInputsAmount,
            cliArgs.CSVDelimiter
        );
        
        var inputNodes = Enumerable.Range(0, cliArgs.CSVInputsAmount)
            .Select(index => new InputNode(0, index))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new ValueNode(0d), 0.1d},
            {new ValueNode(1d), 0.1d},
            {new ValueNode(2d), 0.1d}
        };
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, 0.2d);
        }
        var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new SumNode(
                children: [inputNodes[0], inputNodes[1], new ValueNode(3d)]
                ), 0.4d}
        };
        // rng for creating first population
        var rng = cliArgs.Seed.HasValue
            ? new Random(cliArgs.Seed.Value)
            : new Random();
        var dummyTreeChromosome = new TreeChromosome(
            new ValueNode(5),
            cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities,
            seed: cliArgs.Seed
        );
        var mutation = new ChangeNodeMutation(
            cliArgs.ChangeNodeMutationProbability,
            percentageToChange: 0.1d,
            terminalNodesProbability=cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            terminalNodes: terminalNodesProbabilities.Keys.ToArray(),
            nonTerminalNodesProbabilities,
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray(),
            seed: cliArgs.Seed
        );

        var outputsAmount = outputs.GetColumnsAmount();
        var GAs = new GeneticAlgorithm<TreeChromosome>[outputsAmount];
        // TODO: create GA for each of the output columns (expecting one-hot encoding)
        for (int outputIndex = 0; outputIndex < outputsAmount; outputIndex++)
        {
            var treeBasedGA = new GeneticAlgorithm<TreeChromosome>(
                createNewFunc: () => dummyTreeChromosome.Clone(
                    rng.NextDouble() < 0.5
                        ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                        : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
                ),
                [ mutation ],
                [ new DummyCrossover() ],
                new AccuracyFitness(
                    inputs,
                    outputs,
                    outputIndex,
                    inputNodes
                ),
                new DummySelection(),
                new TakeNewCombination(),
                callback: (genNum, population) => {
                    System.Console.WriteLine($"Computed {genNum}th generation.");
                }
            );

            GAs[outputIndex] = treeBasedGA;
        }

        System.Console.WriteLine($"{GAs.Length} GAs created.");
    }
    public static bool CheckArgs(Options args)
    {
        return args.CSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
