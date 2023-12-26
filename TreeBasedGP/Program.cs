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
        (double[,] inputs, int[,] outputs) = CSVHelper.PrepareCSV(
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
            ), 0.4d},
            {new ProductNode(
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
                // ramped half-and-half
                createNewFunc: () => dummyTreeChromosome.Clone(
                    rng.NextDouble() < 0.5
                        ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                        : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
                ),
                [mutation],
                [new DummyCrossover()],
                new AccuracyFitness(
                    inputs,
                    outputs,
                    outputIndex,
                    inputNodes
                ),
                new ReversedRouletteWheelSelection<TreeChromosome>(cliArgs.Seed),
                new TakeNewCombination(),
                callback: (genNum, population) =>
                {
                    System.Console.WriteLine($"Computed {genNum}th generation.");
                    // System.Console.WriteLine($"Lowest fitness: {population.Select(ind => ind.Fitness).Min()}");
                    // System.Console.WriteLine($"Representation of lowest fitness: {population.MinBy(ind => ind.Fitness).GetRepresentation()}");
                    // System.Console.WriteLine($"Mean fitness: {population.Select(ind => ind.Fitness).Average()}");
                    // foreach (var ind in population)
                    // {
                    //     System.Console.WriteLine($"{ind.Fitness} ==> {ind}");
                    // }
                    // System.Console.WriteLine();
                    // System.Console.WriteLine($"Highest fitness: {population.Max(ind => ind.Fitness)}");
                }
            ){
                MaxGenerations = cliArgs.MaxGenerations,
                CrossoverProbability = 0d,
                PopulationSize = cliArgs.PopulationSize,
                MutationProbability = 0.3d
            };

            GAs[outputIndex] = treeBasedGA;
        }

        System.Console.WriteLine($"{GAs.Length} GAs created.");

        TreeChromosome[][] resultPopulations = new TreeChromosome[GAs.Length][];
        TreeChromosome[] bestIndividuals = new TreeChromosome[GAs.Length];
        for (int i = 0; i < GAs.Length; i++)
        {
            System.Console.Error.WriteLine($"Running GA number {i}...");
            resultPopulations[i] = GAs[i].StartSingleThreaded();
            System.Console.Error.WriteLine($"GA {i} done.");
        }


        for (int i = 0; i < resultPopulations.Length; i++)
        {
            System.Console.WriteLine($"Best individual for output #{i} (Fitness = {resultPopulations[i].Min(ind => ind.Fitness)}):");
            bestIndividuals[i] = resultPopulations[i].MinBy(ind => ind.Fitness);
            System.Console.WriteLine(bestIndividuals[i].GetRepresentation());
            System.Console.WriteLine();
        }

        System.Console.WriteLine("Calculating prediction accuracy...");

        int goodPredictionCounter = 0;
        foreach ((var row_inputs, var row_outputs) in Enumerable.Zip(inputs.IterateRows(), outputs.IterateRows()))
        {
            foreach ((var inputNode, var inputValue) in Enumerable.Zip(inputNodes, row_inputs))
            {
                inputNode.Update(inputValue);
            }

            double[] predictions = bestIndividuals.Select(ind => ind.ComputeResult()).ToArray();
            double bestPrediction = predictions.Max();
            // choose max as predicted class
            int[] predictedClass = predictions.Select(pred => pred == bestPrediction ? 1 : 0).ToArray();

            if (Enumerable.Zip(predictedClass, row_outputs).All(tup => tup.First == tup.Second))
                goodPredictionCounter += 1;
        }
        double accuracyScore = (double)goodPredictionCounter / inputs.GetRowsAmount();
        System.Console.WriteLine($"Accuracy score: {accuracyScore * 100 :.3f}%");
    }
    public static bool CheckArgs(Options args)
    {
        return args.CSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
