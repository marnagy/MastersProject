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
        var terminalNodesProbabilities = new Dictionary<TreeNode, double>
        {
            { new ValueNode(0), 0.4d }
        };
        for (int i = 1; i <= 5; i++)
        {
            terminalNodesProbabilities.Add(new ValueNode(i), 0.4d);
            terminalNodesProbabilities.Add(new ValueNode(-i), 0.4d);
        }
            // {new ValueNode(0d), 0.4d},
            // {new ValueNode(1d), 0.4d},
            // {new ValueNode(2d), 0.4d}
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, 0.2d);
        }
        TreeNode[] baseChildren = Enumerable.Range(0, TreeNode.ChildrenAmount)
            .Select(i => new ValueNode(-i))
            .ToArray();
        var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new SumNode(baseChildren), 1d},
            {new ProductNode(baseChildren), 1d},
            {new UnaryMinusNode(baseChildren), 1d},
            {new SinNode(baseChildren), 0.5d},
            {new SigmoidNode(baseChildren), 0.2d}
        };
        // rng for creating first population
        var rng = Random.Shared;
        TreeChromosome.DefaultDepth = cliArgs.DefaultTreeDepth;

        var dummyTreeChromosome = new TreeChromosome(
            new ValueNode(5),
            cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities
        );
        var mutation = new ChangeNodeMutation(
            cliArgs.ChangeNodeMutationProbability,
            percentageToChange: 0.1d,
            terminalNodesProbability=cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            terminalNodes: terminalNodesProbabilities.Keys.ToArray(),
            nonTerminalNodesProbabilities,
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray()
        );
        Func<TreeChromosome> newChromosomeFunc = () => dummyTreeChromosome.Clone(
            rng.NextDouble() < 0.5
                ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
        );

        var outputsAmount = outputs.GetColumnsAmount();
        var GAs = new GeneticAlgorithm<TreeChromosome>[outputsAmount];
        // TODO: create GA for each of the output columns (expecting one-hot encoding)
        for (int outputIndex = 0; outputIndex < outputsAmount; outputIndex++)
        {
            var fitness = new AccuracyFitness(
                inputs,
                outputs,
                outputIndex,
                inputNodes
            );
            var treeBasedGA = new GeneticAlgorithm<TreeChromosome>(
                // ramped half-and-half
                newChromosomeFunc,
                [mutation],
                [new DummyCrossover()],
                fitness,
                new ReversedRouletteWheelSelection<TreeChromosome>(),
                //new TakeNewCombination(),
                new ElitismCombination<TreeChromosome>(
                    bestAmount: cliArgs.PopulationSize / 10,
                    newIndividuals: cliArgs.PopulationSize / 10,
                    fitnessFunc: fitness
                ),
                callback: (genNum, population) =>
                {
                    System.Console.WriteLine($"Computed {genNum}th generation. " +
                        $"Lowest Fitness: {population.Select(ind => ind.Fitness).Min()} " +
                        $"Average Fitness: {population.Select(ind => ind.Fitness).Average()} ");
                }
            ){
                MaxGenerations = cliArgs.MaxGenerations,
                CrossoverProbability = 0d,
                PopulationSize = cliArgs.PopulationSize,
                MutationProbability = cliArgs.MutationProbability
            };

            GAs[outputIndex] = treeBasedGA;
        }

        System.Console.Error.WriteLine($"{GAs.Length} GAs created.");

        TreeChromosome[][] resultPopulations = new TreeChromosome[GAs.Length][];
        TreeChromosome[] bestIndividuals = new TreeChromosome[GAs.Length];
        for (int i = 0; i < GAs.Length; i++)
        {
            System.Console.Error.WriteLine($"Running GA number {i}...");
            if (!cliArgs.MultiThreaded)
                resultPopulations[i] = GAs[i].StartSingleThreaded();
            else
                resultPopulations[i] = GAs[i].Start();
            System.Console.Error.WriteLine($"GA {i} done.");
        }

        System.Console.Error.WriteLine();

        for (int i = 0; i < resultPopulations.Length; i++)
        {
            System.Console.Error.WriteLine($"Best individual for output #{i} (Fitness = {resultPopulations[i].Min(ind => ind.Fitness)}):");
            bestIndividuals[i] = resultPopulations[i].MinBy(ind => ind.Fitness);
            System.Console.Error.WriteLine(bestIndividuals[i].GetRepresentation());
            System.Console.Error.WriteLine();
        }

        System.Console.WriteLine("Calculating prediction accuracy...");

        int goodPredictionCounter = 0;
        foreach ((var row_inputs, var row_outputs) in Enumerable.Zip(inputs.IterateRows(), outputs.IterateRows()))
        {
            var output_row = row_outputs.ToArray();
            foreach ((var inputNode, var inputValue) in Enumerable.Zip(inputNodes, row_inputs))
            {
                inputNode.Update(inputValue);
            }

            double[] predictions = bestIndividuals.Select(ind => ind.ComputeResult()).ToArray();
            double bestPrediction = predictions.Max();
            // choose max as predicted class
            int[] predictedClass = predictions.Select(pred => pred == bestPrediction ? 1 : 0).ToArray();
            // System.Console.WriteLine($"Wanted output: {output_row.Stringify()}");
            // System.Console.WriteLine($"Predicted: {predictions.Stringify()}");

            if (Enumerable.Zip(predictedClass, output_row).All(tup => tup.First == tup.Second))
                goodPredictionCounter += 1;
        }
        double accuracyScore = (double)goodPredictionCounter / inputs.GetRowsAmount();
        System.Console.WriteLine($"Accuracy score: {accuracyScore * 100 :0.00} %");
    }
    public static bool CheckArgs(Options args)
    {
        return args.CSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
