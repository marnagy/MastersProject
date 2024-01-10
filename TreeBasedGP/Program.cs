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
            .Select(index => new InputFunctionality(index))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<NodeFunctionality, double>
        {
            { new ValueFunctionality(0), 0.4d }
        };
        for (int i = 1; i <= 5; i++)
        {
            terminalNodesProbabilities.Add(new ValueFunctionality(i), 1d);
            terminalNodesProbabilities.Add(new ValueFunctionality(-i), 1d);
        }
            // {new ValueNode(0d), 0.4d},
            // {new ValueNode(1d), 0.4d},
            // {new ValueNode(2d), 0.4d}
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, 0.5d);
        }
        var nonTerminalNodesProbabilities = new Dictionary<NodeFunctionality, double> {
            {new ConditionNode(), 0.2d},
            {new SumNode(), 1d},
            {new ProductNode(), 1d},
            // {new PowerNode(), 0.1d},
            {new UnaryMinusNode(), 1d},
            {new SinNode(), 0.5d},
            // {new SigmoidNode(), 0.2d}
        };
        // rng for creating first population
        var rng = Random.Shared;
        TreeChromosome.DefaultDepth = cliArgs.DefaultTreeDepth;

        var dummyTreeChromosome = new TreeChromosome(
            new TreeNodeMaster(new ValueFunctionality(0d), children: null),
            cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities
        );
        // var secondChromosome = dummyTreeChromosome.Clone();

        // int a = 5;

        // return;
        var mutationChange = new ChangeNodeMutation(
            cliArgs.ChangeNodeMutationProbability,
            percentageToChange: 0.2d,
            terminalNodesProbability=cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            terminalNodes: terminalNodesProbabilities.Keys.ToArray(),
            nonTerminalNodesProbabilities,
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray()
        );
        // var mutationShuffle = new SwitchChildrenMutation(
        //     cliArgs.ChangeNodeMutationProbability,
        //     percentageToChange: 0.1d
        // );
        Mutation<TreeChromosome>[] mutations = [mutationChange]; //, mutationShuffle];
        Func<TreeChromosome> newChromosomeFunc = () => dummyTreeChromosome.Clone(
            rng.NextDouble() < 0.5
                ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
        );

        var outputsAmount = outputs.GetColumnsAmount();
        double previousMinFitness = -1d;
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
                mutations,
                [new DummyCrossover()],
                //[new SwitchNodesCrossover()],
                fitness,
                new ReversedRouletteWheelSelection<TreeChromosome>(),
                //new TakeNewCombination(),
                new ElitismCombination<TreeChromosome>(
                    bestAmount: 1,
                    newIndividuals: cliArgs.PopulationSize / 10,
                    fitnessFunc: fitness
                ),
                callback: (genNum, population) =>
                {
                    var currentMinFitness = population
                        // .Where(ind => double.IsNormal(ind.Fitness))
                        .Select(ind => ind.Fitness)
                        .Min();
                    var currentAvgFitness = population
                        // .Where(ind => double.IsNormal(ind.Fitness))
                        .Select(ind => ind.Fitness)
                        .Average();

                    if (currentMinFitness > previousMinFitness)
                        throw new Exception("Weird elitism...");
                    
                    previousMinFitness = currentMinFitness;

                    System.Console.WriteLine($"Computed {genNum}th generation. " +
                        $"Lowest Fitness: {currentMinFitness} " +
                        $"Average Fitness: {currentAvgFitness} ");
                    // System.Console.WriteLine(population.Select(ind => ind.Fitness).Stringify());
                }
            ){
                MaxGenerations = cliArgs.MaxGenerations,
                CrossoverProbability = 0.8d,
                PopulationSize = cliArgs.PopulationSize,
                MutationProbability = cliArgs.MutationProbability,
                MinThreads = cliArgs.MinThreads,
                MaxThreads = cliArgs.MaxThreads
            };

            GAs[outputIndex] = treeBasedGA;
        }

        System.Console.Error.WriteLine($"{GAs.Length} GAs created.");

        TreeChromosome[][] resultPopulations = new TreeChromosome[GAs.Length][];
        TreeChromosome[] bestIndividuals = new TreeChromosome[GAs.Length];
        for (int i = 0; i < GAs.Length; i++)
        {
            int gaNum = i+1;
            System.Console.Error.WriteLine($"Running GA number {gaNum}...");
            if (!cliArgs.MultiThreaded)
                resultPopulations[i] = GAs[i].StartSingleThreaded();
            else
                resultPopulations[i] = GAs[i].Start();
            System.Console.Error.WriteLine($"GA {gaNum} done.");
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
                inputNode.Value = inputValue;
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
