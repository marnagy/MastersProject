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

        Options cliArgsMut = Parser.Default.ParseArguments<Options>(args).Value;
        if (cliArgsMut == null)  // --help case
            return;
        
        var cliArgs = OptionsImmutable.From(cliArgsMut);

        if (!CheckArgs(cliArgs))
        {
            System.Console.Error.WriteLine("Invalid arguments.");
            System.Console.Error.WriteLine(cliArgs);
        }

        double terminalNodesProbability = cliArgs.TerminalNodesProbability;

        // prepare CSV
        (double[,] inputs, int[,] outputs) = CSVHelper.PrepareCSV(
            cliArgs.TrainCSVFilePath,
            cliArgs.CSVInputsAmount,
            cliArgs.CSVDelimiter
        );
        
        var inputNodes = Enumerable.Range(0, cliArgs.CSVInputsAmount)
            .Select(index => new InputFunctionality(index))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<NodeFunctionality, double>
        {
            {new ValueFunctionality(0d), cliArgs.ValueNodeProbability}
        };
        for (int i = 1; i <= 5; i++)
        {
            terminalNodesProbabilities.Add(new ValueFunctionality(i), cliArgs.ValueNodeProbability);
            //terminalNodesProbabilities.Add(new ValueFunctionality(-i), cliArgs.ValueNodeProbability);
        }
        // also add input nodes so they can be used wherever in trees
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, cliArgs.InputNodeProbability);
        }
        var nonTerminalNodesProbabilities = new Dictionary<NodeFunctionality, double>
        {
            // tertiary
            {new ConditionNode(), cliArgs.ConditionNodeProbability},
            // binary
            {new SumNode(), cliArgs.SumNodeProbability},
            {new ProductNode(), cliArgs.ProductNodeProbability},
            {new PowerNode(), cliArgs.PowerNodeProbability},
            // unary
            {new UnaryMinusNode(), cliArgs.UnaryMinusNodeProbability},
            {new SinNode(), cliArgs.SinNodeProbability},
            {new ReLUNode(), cliArgs.ReLUNodeProbability},
            {new SigmoidNode(), cliArgs.SigmoidNodeProbability}
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
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray(),
            maxThreads: cliArgs.MaxThreads
        );
        var mutationShuffle = new ShuffleChildrenCombinedMutation(
            cliArgs.ShuffleChildrenMutationProbability,
            percentageToChange: cliArgs.PercentageToChange
        );
        Mutation<CombinedTreeChromosome>[] mutations = [
            mutationChange,
            mutationShuffle
        ];
        // ramped half-and-half
        Func<TreeChromosome> newTreeChromosome = () => 
        dummyTreeChromosome.Clone(
            Random.Shared.NextDouble() < 0.5
                ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
        );

        Func<CombinedTreeChromosome> createNewChromosome = () => CombinedTreeChromosome.CreateNew(
            outputs.GetColumnsAmount(),
            newTreeChromosome
        );
        Crossover<CombinedTreeChromosome>[] crossovers = [
            new CombinedSwitchNodesCrossover()
        ];

        var outputsAmount = outputs.GetColumnsAmount();
        double previousMinFitness = double.PositiveInfinity;
        var trainAccuracy = new CombinedAccuracyFitness(
            inputs,
            outputs,
            inputNodes,
            cliArgs.MaxThreads
        );
        PopulationCombinationStrategy<CombinedTreeChromosome> popComb = cliArgs.PopulationCombination switch
        {
            "elitism" => new MinElitismCombination<CombinedTreeChromosome>(
                bestAmount: 1,
                newIndividuals: 0,
                trainAccuracy,
                createNewChromosome
            ),
            "take-new" => new TakeNewCombination<CombinedTreeChromosome>(),
            "combine" => new MinCombineBestCombination<CombinedTreeChromosome>(),
            _ => throw new Exception("User should not be able to get here. Default handling in OptionsImmutable class"),
        };

        //!  ##### ! #####
        var combinedTreeBasedGA = new GeneticAlgorithm<CombinedTreeChromosome>(
            createNewChromosome,
            mutations,
            crossovers,
            trainAccuracy,
            new MinTournamentSelection<CombinedTreeChromosome>(5),
            popComb
        ){
            MaxGenerations = cliArgs.MaxGenerations,
            CrossoverProbability = cliArgs.CrossoverProbability,
            PopulationSize = cliArgs.PopulationSize,
            MinThreads = cliArgs.MinThreads,
            MaxThreads = cliArgs.MaxThreads
        };

        CombinedTreeChromosome.MaxThreads = cliArgs.MaxThreads;

        var dt = DateTime.UtcNow;

        var masterDirectory = Directory.CreateDirectory($"combined-scored_{dt.Year}-{dt.Month:00}-{dt.Day:00}_{dt.Hour:00}-{dt.Minute:00}-{dt.Second:00}");
        var baseDirectory = masterDirectory;

        //save cliArgs
        File.WriteAllText(
            Path.Combine(masterDirectory.FullName, "args.json"),
            JsonSerializer.Serialize(cliArgs, new JsonSerializerOptions(){
                WriteIndented=true
            })
        );

        File.WriteAllText(
            Path.Combine(masterDirectory.FullName, "info.txt"),
            $"Computed using CSV {cliArgs.TrainCSVFilePath} with {outputsAmount} output classes."
        );

        for (int j = 0; j < cliArgs.RepeatAmount; j++)
        {
            if (cliArgs.RepeatAmount > 1)
                baseDirectory = masterDirectory.CreateSubdirectory($"run_{j}");

            CombinedTreeChromosome[] resultPopulation; // = new CombinedTreeChromosome[cliArgs.PopulationSize];
            CombinedTreeChromosome bestIndividual;
            System.Console.Error.WriteLine($"Running GA...");
            // previousMinFitness = double.PositiveInfinity;
            string[] columnNames = [
                "gen",
                "minFitness",
                "averageFitness",
                "minScore",
                "averageScore",
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
                    
                    var currentMinScore = population
                        .Select(ind => ind.Score)
                        .Min();
                    var currentAvgScore = population
                        // fitness can be +inf
                        .Where(ind => double.IsNormal(ind.Score) || ind.Score == 0d)
                        .Select(ind => ind.Score)
                        .Average();

                    // if (currentMinFitness > previousMinFitness)
                    //     throw new Exception("Weird elitism...");
                    
                    // previousMinFitness = currentMinFitness;

                    var depthsFunc = population.Select(ind => ind.GetDepth());
                    var minDepth = depthsFunc.Min();
                    var averageDepth = depthsFunc.Average();
                    sw.WriteLine(string.Join(',', new[]{
                        genNum,
                        currentMinFitness,
                        currentAvgFitness,
                        currentMinScore,
                        currentAvgScore,
                        minDepth,
                        averageDepth
                    }));

                    System.Console.Error.WriteLine($"Computed {genNum}th generation. " +
                        $"MinFitness: {currentMinFitness} " +
                        $"AvgFitness: {currentAvgFitness} " + //:F2} " +
                        $"MinScore: {currentMinScore:F3} " +
                        $"AvgScore: {currentAvgScore:F3} " +
                        $"Depth of min: {population.MinBy(ind => ind.Fitness).GetDepth():F1} " +
                        $"AvgDepth: {averageDepth:F1} "
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
                bestIndividual = resultPopulation
                    .Where(ind => double.IsNormal(ind.Fitness) || ind.Fitness == 0d)
                    .MinBy(ind => ind.Fitness);
                sw.WriteLine(bestIndividual.GetRepresentation());
                System.Console.Error.WriteLine(bestIndividual.GetRepresentation());
                System.Console.Error.WriteLine();

                System.Console.WriteLine("Calculating prediction accuracy...");

                // load test inputs & outputs if specified
                if (cliArgs.TestCSVFilePath != null)
                    (inputs, outputs) = CSVHelper.PrepareCSV(
                        cliArgs.TrainCSVFilePath,
                        cliArgs.CSVInputsAmount,
                        cliArgs.CSVDelimiter
                    );

                var resultAccuracyFitness = new CombinedAccuracyFitness(inputs, outputs, inputNodes, cliArgs.MaxThreads);

                double accuracyScore = 1d - resultAccuracyFitness.ComputeFitness(bestIndividual);
                
                System.Console.Error.WriteLine($"Accuracy score: {accuracyScore * 100 } %");
                sw.WriteLine($"Accuracy score: {accuracyScore * 100} %");
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

    public static bool CheckArgs(OptionsImmutable cliArgs)
    {
        return cliArgs.TrainCSVFilePath != null
            && cliArgs.CSVInputsAmount > 0;
    }
}
