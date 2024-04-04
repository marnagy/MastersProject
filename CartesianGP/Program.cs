using System.Globalization;
using System.Text;
using System.Text.Json;
using CommandLine;

class Program
{
    public static void Main(string[] args)
    {
        // set to en-us culture -> interpret real number with decimal point instead of decimal comma
        // from https://stackoverflow.com/questions/2234492/is-it-possible-to-set-the-cultureinfo-for-an-net-application-or-just-a-thread#comment32681459_2247570
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        Options cliArgsMut = Parser.Default.ParseArguments<Options>(args).Value;
        //System.Console.WriteLine(cliArgs);
        if (cliArgsMut == null) // --help case
            return;
        
        var cliArgs = OptionsImmutable.From(cliArgsMut);
        
        if (!CheckArgs(cliArgs))
        {
            System.Console.Error.WriteLine("Invalid arguments.");
            System.Console.Error.WriteLine(cliArgs);
        }

        System.Console.Error.WriteLine(cliArgs);

        double terminalNodesProbability = cliArgs.TerminalNodesProbability;

        // prepare CSV
        (double[,] inputs, int[,] outputs) = CSVHelper.PrepareCSV(
            cliArgs.TrainCSVFilePath,
            cliArgs.CSVInputsAmount,
            cliArgs.CSVDelimiter
        );
        int inputsAmount = inputs.GetRow(0).Count();
        int outputsAmount = outputs.GetRow(0).Count();

        var emptyParents = CartesianNode.GetEmptyParents();
        var terminalNodesProbabilities = new Dictionary<CartesianNode, double>
        {
            {new ValueNode(0d, emptyParents), cliArgs.ValueNodeProbability}
        };
        for (int i = 1; i <= 2; i++)
        {
            terminalNodesProbabilities.Add(
                new ValueNode(i, emptyParents), cliArgs.ValueNodeProbability
            );
        }
        // no need to add input nodes

        var nonTerminalNodesProbabilities = new Dictionary<CartesianNode, double>
        {
            // tertiary
            // {new ConditionNode(emptyParents), cliArgs.ConditionNodeProbability},
            // binary
            {new SumNode(emptyParents), cliArgs.SumNodeProbability},
            {new ProductNode(emptyParents), cliArgs.ProductNodeProbability},
            // {new PowerNode(emptyParents), cliArgs.PowerNodeProbability},
            // unary
            {new UnaryMinusNode(emptyParents), cliArgs.UnaryMinusNodeProbability},
            {new SinNode(emptyParents), cliArgs.SinNodeProbability},
            {new ReLUNode(emptyParents), cliArgs.ReLUNodeProbability},
            {new SigmoidNode(emptyParents), cliArgs.SigmoidNodeProbability}

        };

        var trainAccuracy = new AccuracyFitness(
            inputs,
            outputs,
            cliArgs.MaxThreads
        );

        AccuracyFitness testAccuracy;
        if (cliArgs.TestCSVFilePath == null)
        {
            testAccuracy = trainAccuracy;
        }
        else
        {
            (double[,] testInputs, int[,] testOutputs) = CSVHelper.PrepareCSV(
                cliArgs.TestCSVFilePath,
                cliArgs.CSVInputsAmount,
                cliArgs.CSVDelimiter
            );
            testAccuracy = new AccuracyFitness(
                testInputs,
                testOutputs,
                cliArgs.MaxThreads
            );

        }
        int[] hiddenlayerSizes = cliArgs.LayerSizes.ToArray();
        int[] layerSizes = new int[1 + hiddenlayerSizes.Length + 1];
        layerSizes[0] = inputsAmount;
        layerSizes[^1] = outputsAmount;
        hiddenlayerSizes.CopyTo(layerSizes, 1);

        Func<CartesianChromosome> createNewChromosome = ()
            => CartesianChromosome.CreateNewRandom(
                layerSizes,
                cliArgs.TerminalNodesProbability,
                terminalNodesProbabilities,
                nonTerminalNodesProbabilities
            );
        var cartesianGA = new GeneticAlgorithm<CartesianChromosome>(
            createNewChromosome,
            [ 
                new ChangeNodeMutation(
                    cliArgs.PercentageToChange,
                    cliArgs.ChangeNodeMutationProbability,
                    cliArgs.TerminalNodesProbability,
                    nonTerminalNodesProbabilities,
                    terminalNodesProbabilities
                ),
                new ChangeParentsMutation(
                    cliArgs.PercentageToChange,
                    cliArgs.ChangeParentsMutationProbability
                )
            ],
            [new FixedIndexCrossover()],
            trainAccuracy,
            new MinTournamentSelection<CartesianChromosome>(folds: 5),
            new MinElitismCombination<CartesianChromosome>(
                bestAmount: 1,
                newIndividuals: 0,
                trainAccuracy,
                createNewChromosome
            )
            // new TakeNewCombination<CartesianChromosome>()
            // new MinCombineBestCombination<CartesianChromosome>()
        ){
            CrossoverProbability = cliArgs.CrossoverProbability,
            MaxGenerations = cliArgs.MaxGenerations,
            PopulationSize = cliArgs.PopulationSize,
            MinThreads = cliArgs.MinThreads,
            MaxThreads = cliArgs.MaxThreads
        };

        var dt = DateTime.UtcNow;
        var masterDirectory = Directory.CreateDirectory(
            $"cartesian-{dt.Year}-{dt.Month:00}-{dt.Day:00}_" + 
            $"{dt.Hour:00}-{dt.Minute:00}-{dt.Second:00}"
        );
        var baseDirectory = masterDirectory;
        File.WriteAllText(
            Path.Combine(masterDirectory.FullName, "info.txt"),
            $"Computed using CSV {cliArgs.TrainCSVFilePath} with {outputsAmount} output classes."
        );

        File.WriteAllText(
            Path.Combine(masterDirectory.FullName, "config.json"),
            JsonSerializer.Serialize(cliArgs, new JsonSerializerOptions(){
                WriteIndented = true,
                IgnoreReadOnlyFields = false
            }),
            Encoding.UTF8
        );

        for (int j = 0; j < cliArgs.RepeatAmount; j++)
        {
            if (cliArgs.RepeatAmount > 1)
                baseDirectory = masterDirectory.CreateSubdirectory($"run_{j}");
            
            CartesianChromosome[] resultPopulation;
            CartesianChromosome bestIndividual;
            System.Console.Error.WriteLine($"Running GA...");
            string[] columnNames = [
                "gen",
                "minFitness",
                "averageFitness",
                // "minScore",
                // "averageScore",
                "minDepth",
                "averageDepth",
            ];
            using (var sw = new StreamWriter(File.OpenWrite(Path.Combine(baseDirectory.FullName, $"run.csv"))))
            {
                sw.WriteLine(string.Join(',', columnNames));
                Action<int, IReadOnlyList<CartesianChromosome>> callback = (genNum, population) =>
                {
                    double currentMinFitness = population
                        .Select(ind => ind.Fitness)
                        .Min();
                    double currentAvgFitness = population
                        // fitness can be +inf
                        .Where(ind => double.IsNormal(ind.Fitness) || ind.Fitness == 0d)
                        .Select(ind => ind.Fitness)
                        .Average();
                    
                    int[] depths = population
                        .Select(ind => ind.GetDepth())
                        .ToArray();
                    int minDepth = depths.Min();
                    double averageDepth = depths.Average();
                    sw.WriteLine(string.Join(',', new[]{
                        genNum,
                        currentMinFitness,
                        currentAvgFitness,
                        // currentMinScore,
                        // currentAvgScore,
                        minDepth,
                        averageDepth
                    }));

                    if (genNum % 10 == 0)
                        System.Console.Error.WriteLine($"Computed {genNum}th generation. " +
                            $"MinFitness: {currentMinFitness} " +
                            $"AvgFitness: {currentAvgFitness} " + //:F2} " +
                            // $"MinScore: {currentMinScore:F3} " +
                            // $"AvgScore: {currentAvgScore:F3} " +
                            $"Depth of min: {population.First(ind => ind.Fitness == currentMinFitness).GetDepth():F1} " +
                            $"AvgDepth: {averageDepth:F1} "
                        );
                };
                Func<IReadOnlyList<CartesianChromosome>, bool> stopCond = (population)
                    => false;
                resultPopulation = cliArgs.MultiThreaded
                    ? cartesianGA.Start(callback, stopCond)
                    : cartesianGA.StartSingleThreaded(callback, stopCond);
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

                double accuracyScore = 1d - testAccuracy.ComputeFitness(bestIndividual);
                
                System.Console.Error.WriteLine($"Accuracy score: {accuracyScore * 100 :F2} %");
                sw.WriteLine($"Accuracy score: {accuracyScore * 100} %");
            }
        }
    }

    private static bool CheckArgs(OptionsImmutable cliArgs)
    {
        return cliArgs.CSVInputsAmount > 0
            && cliArgs.MinThreads >= 1
            && cliArgs.MaxThreads >= 1
            && cliArgs.MaxThreads >= cliArgs.MinThreads
            // technically only positive is needed, but come on...
            && cliArgs.PopulationSize >= 10 
            && cliArgs.PopulationSize % 2 == 0 // needed for crossover
            && cliArgs.MaxGenerations >= 100
            && cliArgs.RepeatAmount >= 1
            && cliArgs.LayerSizes != null
            // all probabilities have to be non-negative
            && cliArgs.CrossoverProbability >= 0d
            && cliArgs.ChangeNodeMutationProbability >= 0d
            && cliArgs.ChangeParentsMutationProbability >= 0d
            && cliArgs.PercentageToChange >= 0d
            && cliArgs.TerminalNodesProbability >= 0d
            && cliArgs.ValueNodeProbability >= 0d
            && cliArgs.SumNodeProbability >= 0d
            && cliArgs.ProductNodeProbability >= 0d
            && cliArgs.SinNodeProbability >= 0d
            && cliArgs.PowerNodeProbability >= 0d
            && cliArgs.UnaryMinusNodeProbability >= 0d
            && cliArgs.SigmoidNodeProbability >= 0d
            && cliArgs.ReLUNodeProbability >= 0d
            && cliArgs.ConditionNodeProbability >= 0d
            && cliArgs.InputNodeProbability >= 0d;
    }
}
