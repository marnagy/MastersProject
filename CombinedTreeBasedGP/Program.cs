using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
using Microsoft.VisualBasic;

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
            {new ValueFunctionality(0d), 0.2d}
        };
        for (int i = 1; i <= 1; i++)
        {
            terminalNodesProbabilities.Add(new ValueFunctionality(i), 0.2d);
            //terminalNodesProbabilities.Add(new ValueFunctionality(-i), 0.2d);
        }
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, 1d);
        }
        var nonTerminalNodesProbabilities = new Dictionary<NodeFunctionality, double> {
            // {new ConditionNode(), 0.5d},
            {new SumNode(), 1d},
            {new ProductNode(), 1d},
            // {new PowerNode(), 0.1d},
            {new UnaryMinusNode(), 0.7d},
            {new SinNode(), 0.5d},
            // {new SigmoidNode(), 0.2d}
        };
        // rng for creating first population
        TreeChromosome.DefaultDepth = cliArgs.DefaultTreeDepth;

        var dummyTreeChromosome = new TreeChromosome(
            new TreeNodeMaster(new ValueFunctionality(0d), children: null),
            cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities
        );

        var mutationChange = new ChangeNodesMutation(
            cliArgs.ChangeNodeMutationProbability,
            outputs.GetColumnsAmount(),
            percentageToChange: cliArgs.PercentageToChange,
            terminalNodesProbability:cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            terminalNodes: terminalNodesProbabilities.Keys.ToArray(),
            nonTerminalNodesProbabilities,
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray()
        );
        // var mutationShuffle = new ShuffleChildrenMutation(
        //     cliArgs.ChangeNodeMutationProbability,
        //     percentageToChange: cliArgs.PercentageToChange
        // );
        Mutation<CombinedTreeChromosome>[] mutations = [
            mutationChange,
            //mutationShuffle,
        ];
        // ramped half-and-half
        Func<TreeChromosome> newTreeChromosome = () => 
            dummyTreeChromosome.CreateNew();
        // dummyTreeChromosome.Clone(
        //     Random.Shared.NextDouble() < 0.5
        //         ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
        //         : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
        //     // dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
        // );
        Func<CombinedTreeChromosome> newChromosomeFunc = () => CombinedTreeChromosome.CreateNew(
            outputs.GetColumnsAmount(),
            newTreeChromosome
        );
        Crossover<CombinedTreeChromosome>[] crossovers = [
            // new CombinedSwitchNodesCrossover(),
            new DummyCombinedCrossover(), 
        ];

        var outputsAmount = outputs.GetColumnsAmount();
        double previousMinFitness = double.PositiveInfinity;
        var fitness = 
        //new CombinedDifferenceFitness(
        new CombinedAccuracyFitness(
            inputs,
            outputs,
            inputNodes
        );

        //!  ##### ! #####
        var combinedTreeBasedGA = new GeneticAlgorithm<CombinedTreeChromosome>(
            newChromosomeFunc,
            mutations,
            crossovers,
            fitness,
            //new ReversedRouletteWheelSelection<CombinedTreeChromosome>(),
            new MinTournamentSelection<CombinedTreeChromosome>(5),
            new TakeNewCombination<CombinedTreeChromosome>()
            // new MinElitismCombination<CombinedTreeChromosome>(
            //     bestAmount: 2,
            //     newIndividuals: 0, // cliArgs.PopulationSize / 10,
            //     fitnessFunc: fitness,
            //     createNewChrom: newChromosomeFunc
            // )
            // new MinCombineBestCombination<CombinedTreeChromosome>()
        ){
            MaxGenerations = cliArgs.MaxGenerations,
            CrossoverProbability = cliArgs.CrossoverProbability,
            PopulationSize = cliArgs.PopulationSize,
            MutationProbability = cliArgs.MutationProbability,
            MinThreads = cliArgs.MinThreads,
            MaxThreads = cliArgs.MaxThreads
        };

        var dt = DateTime.UtcNow;

        var masterDirectory = Directory.CreateDirectory($"combined_{dt.Year}-{dt.Month}-{dt.Day}_{dt.Hour}-{dt.Minute}-{dt.Second}");
        var baseDirectory = masterDirectory;

        //save cliArgs
        File.WriteAllText(
            Path.Combine(masterDirectory.FullName, "args.json"),
            JsonSerializer.Serialize(cliArgs, new JsonSerializerOptions(){
                WriteIndented=true
            })
        );

        for (int j = 0; j < cliArgs.RepeatAmount; j++)
        {
            if (cliArgs.RepeatAmount > 1)
                baseDirectory = masterDirectory.CreateSubdirectory($"run_{j}");

            CombinedTreeChromosome[] resultPopulation; // = new CombinedTreeChromosome[cliArgs.PopulationSize];
            CombinedTreeChromosome bestIndividual;
            System.Console.Error.WriteLine($"Running GA...");
            previousMinFitness = double.PositiveInfinity;
            string[] columnNames = [
                "gen",
                "minFitness",
                "averageFitness",
                "minDepth",
                "averageDepth",
            ];
            using (var sw = new StreamWriter(File.OpenWrite(Path.Combine(baseDirectory.FullName, $"run.csv"))))
            {
                sw.WriteLine(string.Join(',', columnNames));
                Action<int, IReadOnlyList<CombinedTreeChromosome>> callback = (genNum, population) =>
                {
                    var currentMinFitness = population
                        .Select(ind => ind.Fitness)
                        .Min();
                    var currentAvgFitness = population
                        // fitness can be +inf
                        .Where(ind => double.IsNormal(ind.Fitness) || ind.Fitness == 0d)
                        .Select(ind => ind.Fitness)
                        .Average();

                    // if (currentMinFitness > previousMinFitness)
                    //     throw new Exception("Weird elitism...");
                    
                    previousMinFitness = currentMinFitness;

                    var depthsFunc = population.Select(ind => ind.GetDepth());
                    var minDepth = depthsFunc.Min();
                    var averageDepth = depthsFunc.Average();
                    sw.WriteLine(string.Join(',', new[]{
                        genNum,
                        currentMinFitness,
                        currentAvgFitness,
                        minDepth,
                        averageDepth
                    }));

                    System.Console.Error.WriteLine($"Computed {genNum}th generation. " +
                        $"Lowest Fitness: {currentMinFitness} " +
                        $"Average Fitness: {1/currentAvgFitness} " + //:F2} " +
                        $"Depth of min: {population.MinBy(ind => ind.Fitness).GetDepth()} " +
                        $"Average depth: {averageDepth:F1} "
                    );
                    // System.Console.WriteLine(population.Select(ind => ind.Fitness).Stringify());
                };
                double smallDelta = 1d / inputs.GetRowsAmount();
                Func<IReadOnlyList<CombinedTreeChromosome>, bool> stopCond = (population)
                    => false; // population.Min(ind => ind.Fitness) <= smallDelta;
                resultPopulation = cliArgs.MultiThreaded
                    ? combinedTreeBasedGA.Start(callback, stopCond)
                    : combinedTreeBasedGA.StartSingleThreaded (callback, stopCond);
            }
            System.Console.Error.WriteLine("GA done.");

            System.Console.Error.WriteLine();

            using (var sw = new StreamWriter(File.OpenWrite(Path.Combine(baseDirectory.FullName, "result_formulas.txt"))))
            {
                System.Console.Error.WriteLine($"Best individual (Fitness = {resultPopulation.Min(ind => ind.Fitness)}):");
                bestIndividual = resultPopulation.MinBy(ind => ind.Fitness);
                sw.WriteLine(bestIndividual.GetRepresentation());
                System.Console.Error.WriteLine(bestIndividual.GetRepresentation());
                System.Console.Error.WriteLine();

                System.Console.WriteLine("Calculating prediction accuracy...");

                int goodPredictionCounter = 0;
                foreach ((var row_inputs, var row_outputs) in Enumerable.Zip(inputs.IterateRows(), outputs.IterateRows()))
                {
                    var output_row = row_outputs.ToArray();
                    foreach ((var inputNode, var inputValue) in Enumerable.Zip(inputNodes, row_inputs))
                    {
                        inputNode.Value = inputValue;
                    }

                    double[] predictions = bestIndividual
                        .ComputeResult()
                        .ToArray();
                    
                    int[] predictionsOneHot = GetOneHotEncoding(predictions);

                    if (Enumerable.Zip(predictions, output_row)
                            .All(tup => tup.First == tup.Second))
                        goodPredictionCounter += 1;
                }
                double accuracyScore = (double)goodPredictionCounter / inputs.GetRowsAmount();
                System.Console.Error.WriteLine($"Accuracy score: {accuracyScore * 100 :0.00} %");
                sw.WriteLine($"Accuracy score: {accuracyScore * 100 :0.00} %");
            }
        }

    }

    private static int[] GetOneHotEncoding(double[] predictions)
    {
        double maxValue = predictions.Max();
        int maxValueIndex = Enumerable.Range(0, predictions.Length)
            .First(i => predictions[i] == maxValue);
        return Enumerable.Range(0, predictions.Length)
            .Select(i => i == maxValueIndex ? 1 : 0)
            .ToArray();
    }

    public static bool CheckArgs(Options args)
    {
        return args.CSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
