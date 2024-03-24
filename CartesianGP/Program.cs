using CommandLine;

class Program
{
    public static void Main(string[] args)
    {
        Options cliArgs = Parser.Default.ParseArguments<Options>(args).Value;
        //System.Console.WriteLine(cliArgs);
        if (cliArgs == null) // --help case
            return;
        
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
        int inputSize = inputs.GetRow(0).Count();
        int outputSize = outputs.GetRow(0).Count();

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
            {new ConditionNode(emptyParents), cliArgs.ConditionNodeProbability},
            // binary
            {new SumNode(emptyParents), cliArgs.SumNodeProbability},
            {new ProductNode(emptyParents), cliArgs.ProductNodeProbability},
            {new PowerNode(emptyParents), cliArgs.PowerNodeProbability},
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

        int[] layerSizes = new int[1 + cliArgs.LayerSizes.Length + 1];
        layerSizes[0] = inputSize;
        layerSizes[^1] = outputSize;
        cliArgs.LayerSizes.CopyTo(layerSizes, 1);

        Func<CartesianChromosome> createNewChromosome = ()
            => CartesianChromosome.CreateNewRandom(
                layerSizes,
                cliArgs.TerminalNodesProbability,
                terminalNodesProbabilities,
                nonTerminalNodesProbabilities
            );
        var cartesianGA = new GeneticAlgorithm<CartesianChromosome>(
            createNewChromosome,
            new[]{ new ChangeNodeMutation(
                    cliArgs.PercentageToChange,
                    cliArgs.MutationProbability,
                    cliArgs.TerminalNodesProbability,
                    nonTerminalNodesProbabilities,
                    terminalNodesProbabilities
                ),
            },
            [new FixedIndexCrossover()],
            trainAccuracy,
            new MinTournamentSelection<CartesianChromosome>(folds: 5),
            new MinElitismCombination<CartesianChromosome>(
                bestAmount: 1,
                newIndividuals: 0,
                trainAccuracy,
                createNewChromosome
            )
        ){
            CrossoverProbability = cliArgs.CrossoverProbability,
            MaxGenerations = cliArgs.MaxGenerations,
            PopulationSize = cliArgs.PopulationSize,
            MinThreads = cliArgs.MinThreads,
            MaxThreads = cliArgs.MaxThreads
        };

        // TODO: implement multiple runs

        // return;

        var test_input = new[] {1,2,3,4,5};
        var inputs = new double[5, 2];

        IReadOnlyDictionary<int, IReadOnlyList<CartesianNode>> nodeCatalogue = new Dictionary<int, IReadOnlyList<CartesianNode>>
        {
            {0, new CartesianNode[] {new ValueNode(0d, CartesianNode.GetEmptyParents())} },
            {2, new CartesianNode[] {new SumNode(CartesianNode.GetEmptyParents()),
                new ProductNode(CartesianNode.GetEmptyParents()) } }
        };

        string csvFilePath;
        int inputsLength;
        AccuracyFitness accuracy;

        bool runBasicTests = false;

        if ( runBasicTests )
        {
            var layerSizes = new[] { 3, 10, 5, 2 };
            var chromosome1 = CartesianChromosome.CreateNewRandom(
                layerSizes,
                nodeCatalogue
            );
            CartesianChromosome chromosome2 = chromosome1.Clone();

            // compare chromosomes
            System.Console.WriteLine($"Are chromosomes the same object? {Object.ReferenceEquals(chromosome1, chromosome2)}");

            System.Console.WriteLine("Chromosome layers tests:");
            for (int i = 0; i < layerSizes.Length; i++)
            {
                System.Console.WriteLine($"Comparing layer {i}...");
                System.Console.WriteLine($"Does chromosome1 layer size correspond to the assignment? {chromosome1[i].Count == layerSizes[i]}");
                System.Console.WriteLine($"Does chromosome2 layer size correspond to the assignment? {chromosome2[i].Count == layerSizes[i]}");
                System.Console.WriteLine($"Are they the same object? {Object.ReferenceEquals(chromosome1[i], chromosome2[i])}");
                for (int j = 0; j < layerSizes[i]; j++)
                {
                    System.Console.WriteLine($"Are the nodes the same object? {Object.ReferenceEquals(chromosome1[i][j], chromosome2[i][j])}");
                    System.Console.WriteLine($"Are nodes the same? {chromosome1[i][j].Equals(chromosome2[i][j])}");
                }
            }

            csvFilePath = @"C:\Users\mnagy\Documents\Matfyz\Diplomka\MastersProject\prepared_Iris.csv";
            inputsLength = 4;
            accuracy = AccuracyFitness.Use(csvFilePath, inputsLength);

            var populationCorrect = Enumerable.Range(0, 10)
                .Select(_ => CartesianChromosome.CreateNewRandom(
                    new[] { inputsLength, 10, 5, 2 },
                    nodeCatalogue
                ))
                .Select(CartesianChromosome.IsValid)
                .ToArray();
                //.All(ind );

            foreach (var boolValue in populationCorrect)
            {
                System.Console.WriteLine(boolValue);
            }
        }

        csvFilePath = @"C:\Users\mnagy\Documents\Matfyz\Diplomka\MastersProject\prepared_Iris.csv";
        inputsLength = 4;
        accuracy = AccuracyFitness.Use(csvFilePath, inputsLength);

        // TODO: implement neccessary functions
        var ga = new GeneticAlgorithm<CartesianChromosome>(
            () => CartesianChromosome.CreateNewRandom(
                new[] { inputsLength, 10, 5, 3, 2 },
                nodeCatalogue
            ),
            new[] {new ChangeNodeMutation(0.2, 0.5, nodeCatalogue)},
            // new[] {new DummyMutation(0.2d)},
            new[] {new FixedIndexCrossover()},
            // new[] {new DummyCrossover()},
            accuracy,
            new RandomFavoredSelection(),
            //new DummySelection(),
            new TakeNewCombination()
        );

        ga.MaxGenerations = 1_000;
        ga.PopulationSize = 100;

        ga.StartSingleThreaded(
            (genNum, population) =>
            {
                System.Console.WriteLine($"Generation {genNum} has finished.");
                System.Console.WriteLine($"Best fitness was {population.Select(ind => ind.Fitness).Max()}");

                // print population
                foreach (var ind in population)
                {
                    System.Console.WriteLine(ind);
                    System.Console.WriteLine();
                }
            },
            _ => false
        );

        // System.Threading.Thread.Sleep(5_000);
    }

    private static bool CheckArgs(Options cliArgs)
    {
        return true;
    }
}
// testing CLI arguments
    // .WithParsed<Options>(o =>
    // {
    //     if (o.Verbose)
    //     {
    //         Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
    //         Console.WriteLine("Quick Start Example! App is in Verbose mode!");
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Current Arguments: -v {o.Verbose}");
    //         Console.WriteLine("Quick Start Example!");
    //     }
    // });


