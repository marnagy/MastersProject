﻿using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
using Microsoft.VisualBasic;

class Program
{
    public static void Main(string[] args)
    {
        System.Console.Error.WriteLine("This project should NOT be run! Run CombinedTreeBasedGP instead.");
        System.Environment.Exit(1);

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
        int inputsAmount = inputs.GetColumnsAmount();
        int outputsAmount = outputs.GetColumnsAmount();
        
        var inputNodes = Enumerable.Range(0, cliArgs.CSVInputsAmount)
            .Select(index => new InputFunctionality(index))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<NodeFunctionality, double>
        {
            {new ValueFunctionality(0d), cliArgs.ValueNodeProbability}
        };
        for (int i = 1; i <= 2; i++)
        {
            terminalNodesProbabilities.Add(new ValueFunctionality(i), cliArgs.ValueNodeProbability);
            terminalNodesProbabilities.Add(new ValueFunctionality(-i), cliArgs.ValueNodeProbability);
        }
        foreach (var inputNode in inputNodes)
        {
            terminalNodesProbabilities.Add(inputNode, cliArgs.InputNodeProbability);
        }
        var nonTerminalNodesProbabilities = new Dictionary<NodeFunctionality, double> {
            {new ConditionNode(), cliArgs.ConditionNodeProbability},
            {new SumNode(), cliArgs.SumNodeProbability},
            {new ProductNode(), cliArgs.ProductNodeProbability},
            {new PowerNode(), cliArgs.PowerNodeProbability},
            {new UnaryMinusNode(), cliArgs.UnaryMinusNodeProbability},
            {new SinNode(), cliArgs.SinNodeProbability},
            {new SigmoidNode(), cliArgs.SigmoidNodeProbability},
            {new ReLUNode(), cliArgs.ReLUNodeProbability}
        };
        TreeChromosome.DefaultDepth = cliArgs.DefaultTreeDepth;

        var dummyTreeChromosome = new TreeChromosome(
            new TreeNodeMaster(new ValueFunctionality(0d), children: null),
            cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities
        );

        var mutationChange = new ChangeNodeMutation(
            cliArgs.ChangeNodeMutationProbability,
            percentageToChange: cliArgs.PercentageToChange,
            terminalNodesProbability=cliArgs.TerminalNodesProbability,
            terminalNodesProbabilities,
            terminalNodes: terminalNodesProbabilities.Keys.ToArray(),
            nonTerminalNodesProbabilities,
            nonTerminalNodes: nonTerminalNodesProbabilities.Keys.ToArray()
        );
        var mutationShuffle = new ShuffleChildrenMutation(
            cliArgs.ChangeNodeMutationProbability,
            percentageToChange: cliArgs.PercentageToChange
        );
        Mutation<TreeChromosome>[] mutations = [
            mutationChange,
            mutationShuffle,
        ];
        // ramped half-and-half
        Func<TreeChromosome> newChromosomeFunc = () => dummyTreeChromosome.Clone(
            Random.Shared.NextDouble() < 0.5
                ? dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                : dummyTreeChromosome.CreateNewTreeGrow(cliArgs.DefaultTreeDepth)
        );
        Crossover<TreeChromosome>[] crossovers = [
            new SwitchNodesCrossover(),
            // new DummyCrossover(), 
        ];

        double previousMinFitness = double.PositiveInfinity;
        var GAs = new GeneticAlgorithm<TreeChromosome>[outputsAmount];

        // create GA for each of the output columns (expecting one-hot encoding)
        for (int outputIndex = 0; outputIndex < outputsAmount; outputIndex++)
        {
            var createNewChrom = () => dummyTreeChromosome.Clone(
                    dummyTreeChromosome.CreateNewTreeFull(cliArgs.DefaultTreeDepth)
                );
            var fitness = new AccuracyFitness(
                inputs,
                outputs,
                outputIndex,
                inputNodes,
                cliArgs.MaxThreads
            );
            PopulationCombinationStrategy<TreeChromosome> popComb = cliArgs.PopulationCombination switch
            {
                "take-new" => new TakeNewCombination<TreeChromosome>(),
                "elitism" => new MinElitismCombination<TreeChromosome>(
                    bestAmount: 1,
                    0,
                    fitness,
                    createNewChrom
                ),
                "combine" => new MinCombineBestCombination<TreeChromosome>(),
                _ => throw new Exception("Unknown population combination. Should not be reachable!")
            };
            var treeBasedGA = new GeneticAlgorithm<TreeChromosome>(
                newChromosomeFunc,
                mutations,
                crossovers,
                fitness,
                new MinTournamentSelection<TreeChromosome>(3),
                popComb
            ){
                MaxGenerations = cliArgs.MaxGenerations,
                CrossoverProbability = cliArgs.CrossoverProbability,
                PopulationSize = cliArgs.PopulationSize,
                MinThreads = cliArgs.MinThreads,
                MaxThreads = cliArgs.MaxThreads
            };

            GAs[outputIndex] = treeBasedGA;
        }

        System.Console.Error.WriteLine($"{GAs.Length} GAs created.");

        var dt = DateTime.UtcNow;


        var masterDirectory = Directory.CreateDirectory($"{dt.Year}-{dt.Month}-{dt.Day}_{dt.Hour}-{dt.Minute}-{dt.Second}");
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

            TreeChromosome[][] resultPopulations = new TreeChromosome[GAs.Length][];
            TreeChromosome[] bestIndividuals = new TreeChromosome[GAs.Length];
            for (int i = 0; i < GAs.Length; i++)
            {
                int gaNum = i+1;
                System.Console.Error.WriteLine($"Running GA number {gaNum}...");
                previousMinFitness = double.PositiveInfinity;
                string[] columnNames = [
                    "gen",
                    "minFitness",
                    "averageFitness",
                    "minScore",
                    "averageScore",
                    "minDepth",
                    "averageDepth",
                ];
                using (var sw = new StreamWriter(File.OpenWrite(Path.Combine(baseDirectory.FullName, $"run_{gaNum}.csv"))))
                {
                    sw.WriteLine(string.Join(',', columnNames));
                    Action<int, IReadOnlyList<TreeChromosome>> callback = (genNum, population) =>
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
                            $"Average Fitness: {currentAvgFitness:F2} " +
                            $"Depth of min: {population.MinBy(ind => ind.Fitness).GetDepth()} " +
                            $"Average depth: {averageDepth:F1} "
                        );
                        // System.Console.WriteLine(population.Select(ind => ind.Fitness).Stringify());
                    };
                    double smallDelta = Math.Pow(10, -20);
                    Func<IReadOnlyList<TreeChromosome>, bool> stopCond = (population)
                        => false; // population.Min(ind => ind.Fitness) <= smallDelta;
                    if (cliArgs.MultiThreaded)
                        resultPopulations[i] = GAs[i].Start(callback, stopCond);
                    else
                        resultPopulations[i] = GAs[i].StartSingleThreaded(callback, stopCond);
                }
                System.Console.Error.WriteLine($"GA {gaNum} done.");

            }

            System.Console.Error.WriteLine();

            using (var sw = new StreamWriter(File.OpenWrite(Path.Combine(baseDirectory.FullName, "result_formulas.txt"))))
            {
                for (int i = 0; i < resultPopulations.Length; i++)
                {
                    System.Console.Error.WriteLine($"Best individual for output #{i} (Fitness = {resultPopulations[i].Min(ind => ind.Fitness)}):");
                    bestIndividuals[i] = resultPopulations[i].MinBy(ind => ind.Fitness);
                    sw.WriteLine(bestIndividuals[i].GetRepresentation());
                    System.Console.Error.WriteLine(bestIndividuals[i].GetRepresentation());
                    System.Console.Error.WriteLine();
                }

                System.Console.WriteLine("Calculating prediction accuracy...");

                (inputs, outputs) = CSVHelper.PrepareCSV(
                    cliArgs.TestCSVFilePath,
                    cliArgs.CSVInputsAmount,
                    cliArgs.CSVDelimiter
                );

                int goodPredictionCounter = 0;
                foreach ((var row_inputs, var row_outputs) in Enumerable.Zip(inputs.IterateRows(), outputs.IterateRows()))
                {
                    var output_row = row_outputs.ToArray();
                    foreach ((var inputNode, var inputValue) in Enumerable.Zip(inputNodes, row_inputs))
                    {
                        inputNode.Value = inputValue;
                    }

                    double[] predictions = bestIndividuals
                        .Select(ind => ind.ComputeResult())
                        // clip
                        .Select(res => {
                            if (res > 1)
                                return 1d;
                            else if (res < 0)
                                return 0d;
                            else
                                return res;
                        })
                        .ToArray();
                    double bestPrediction = predictions.Max();
                    // choose max as predicted class
                    int[] predictedClass = predictions.Select(pred => pred == bestPrediction ? 1 : 0).ToArray();
                    // System.Console.WriteLine($"Wanted output: {output_row.Stringify()}");
                    // System.Console.WriteLine($"Predicted: {predictions.Stringify()}");

                    if (Enumerable.Zip(predictedClass, output_row).All(tup => tup.First == tup.Second))
                        goodPredictionCounter += 1;
                }

                double accuracyScore = (double)goodPredictionCounter / inputs.GetRowsAmount();
                System.Console.Error.WriteLine($"Accuracy score: {accuracyScore * 100 :0.00} %");
                sw.WriteLine($"Accuracy score: {accuracyScore * 100 :0.00} %");
            }
        }

    }
    public static bool CheckArgs(OptionsImmutable args)
    {
        return args.TrainCSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
