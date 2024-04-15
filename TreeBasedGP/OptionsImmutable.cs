using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(type: typeof(OptionsImmutable))]
public class OptionsImmutable
{
    // default values
    public const bool MultiThreadedDefault = false;
    public const string MultiThreadedDefaultString = "false";
    public const string? TestCSVFilePathDefault = null;
    public const char CSVDelimiterDefault = ',';
    public const string CSVDelimiterDefaultString = ",";
    public const int MinThreadsDefault = 2;
    public const string MinThreadsDefaultString = "2";
    public const int MaxThreadsDefault = 4;
    public const string MaxThreadsDefaultString = "4";
    public const int PopulationSizeDefault = 50;
    public const string PopulationSizeDefaultString = "50";
    public const int MaxGenerationsDefault = 500;
    public const string MaxGenerationsDefaultString = "500";
    public const int RepeatAmountDefault = 5;
    public const string RepeatAmountDefaultString = "5";
    public const double CrossoverProbabilityDefault = 0.4d;
    public const string CrossoverProbabilityDefaultString = "0.4";
    public static readonly IReadOnlyList<string> PopulationCombinationOptions = ["elitism", "take-new", "combine"];
    public const string PopulationCombinationDefault = "take-new";
    public const double MutationProbabilityDefault = 0d;
    public const string MutationProbabilityDefaultString = "0";
    // public const double ChangeNodeMutationProbabilityDefault = 0d;
    // public const double ChangeParentsMutationProbabilityDefault = 0d;
    // public const double AddNodeToLayerMutationProbabilityDefault = 0d;
    // public const double AddLayerMutationProbabilityDefault = 0d;
    // public const double RemoveNodeFromLayerMutationProbabilityDefault = 0d;
    // public const double RemoveLayerMutationProbabilityDefault = 0d;
    public const double PercentageToChangeDefault = 0.2d;
    public const string PercentageToChangeDefaultString = "0.2";
    public const double NodeProbabilityDefault = 0.2d;
    public const string NodeProbabilityDefaultString = "0.2";
    public const double TerminalNodesProbabilityDefault = 0.2d;
    public const string TerminalNodesProbabilityDefaultString = "0.2";
    // public const double ValueNodeProbabilityDefault = 0.2d;
    // public const double SumNodeProbabilityDefault = 0.2d;
    // public const double ProductNodeProbabilityDefault = 0.2d;
    // public const double SinNodeProbabilityDefault = 0.2d;
    // public const double PowerNodeProbabilityDefault = 0.2d;
    // public const double UnaryMinusNodeProbabilityDefault = 0.2d;
    // public const double SigmoidNodeProbabilityDefault = 0.2d;
    // public const double ReLUNodeProbabilityDefault = 0.2d;
    // public const double ConditionNodeProbabilityDefault = 0.2d;

    // general
    public bool MultiThreaded { get; }
    public string TrainCSVFilePath { get; }
    public string? TestCSVFilePath { get; }
    public int CSVInputsAmount { get; }
    public char CSVDelimiter { get; }
    public int MinThreads { get; }
    public int MaxThreads { get; }
    //public readonly int Seed;
    public int MaxGenerations { get; }
    public int PopulationSize { get; }
    // public readonly double MutationProbability;
    public int RepeatAmount { get; }
    public double CrossoverProbability { get; }

    // specific for TreeBasedGP
    public int DefaultTreeDepth { get; }
    public string PopulationCombination { get; }

    // mutation probabilities
    public double ChangeNodeMutationProbability { get; }
    public double ShuffleChildrenMutationProbability { get; }

    // node probabilities
    public double PercentageToChange { get; }
    public double TerminalNodesProbability { get; }
    public double ValueNodeProbability { get; }
    public double SumNodeProbability { get; }
    public double ProductNodeProbability { get; }
    public double SinNodeProbability { get; }
    public double PowerNodeProbability { get; }
    public double UnaryMinusNodeProbability { get; }
    public double SigmoidNodeProbability { get; }
    public double ReLUNodeProbability { get; }
    public double ConditionNodeProbability { get; }
    public double InputNodeProbability { get; }
    [JsonConstructor]
    public OptionsImmutable(
        bool multiThreaded,
        string trainCSVFilePath, // don't use
        string testCSVFilePath, // don't use
        int csvInputsAmount, // don't use
        char csvDelimiter, // don't use
        int minThreads,
        int maxThreads,
        int maxGenerations,
        int populationSize,
        int repeatAmount,
        double crossoverProbability,
        string populationCombination,

        // mutations

        double percentageToChange,
        double terminalNodesProbability,
        double valueNodeProbability,
        double sumNodeProbability,
        double productNodeProbability,
        double sinNodeProbability,
        double powerNodeProbability,
        double unaryMinusNodeProbability,
        double sigmoidNodeProbability,
        double reLUNodeProbability,
        double conditionNodeProbability,
        double inputNodeProbability
    )
    {
        this.MultiThreaded = multiThreaded;
        this.MinThreads = minThreads;
        this.MaxThreads = maxThreads;
        this.MaxGenerations = maxGenerations;
        this.PopulationSize = populationSize;
        this.RepeatAmount = repeatAmount;
        this.CrossoverProbability = crossoverProbability;
        if (!OptionsImmutable.PopulationCombinationOptions.Contains(populationCombination))
        {
            System.Console.Error.WriteLine($"Unknown population combination: {populationCombination}");
            System.Environment.Exit(1);
        }
        this.PopulationCombination = populationCombination;

        // TODO: mutations

        this.PercentageToChange = percentageToChange;
        this.TerminalNodesProbability = terminalNodesProbability;
        this.ValueNodeProbability = valueNodeProbability;
        this.SumNodeProbability = sumNodeProbability;
        this.ProductNodeProbability = productNodeProbability;
        this.SinNodeProbability = sinNodeProbability;
        this.PowerNodeProbability = powerNodeProbability;
        this.UnaryMinusNodeProbability = unaryMinusNodeProbability;
        this.SigmoidNodeProbability = sigmoidNodeProbability;
        this.ReLUNodeProbability = reLUNodeProbability;
        this.ConditionNodeProbability = conditionNodeProbability;
        this.InputNodeProbability = inputNodeProbability;
    }
    private OptionsImmutable(Options opts, OptionsImmutable? fileOpts)
    {
        this.MultiThreaded = opts.MultiThreaded;
        this.TrainCSVFilePath = opts.TrainCSVFilePath;
        this.TestCSVFilePath = opts.TestCSVFilePath ?? OptionsImmutable.TestCSVFilePathDefault;
        this.CSVInputsAmount = opts.CSVInputsAmount;
        this.CSVDelimiter = opts.CSVDelimiter;
        this.MinThreads = opts.MinThreads ?? fileOpts?.MinThreads ?? OptionsImmutable.MinThreadsDefault;
        this.MaxThreads = opts.MaxThreads ?? fileOpts?.MaxThreads ?? OptionsImmutable.MaxThreadsDefault;
        this.PopulationSize = opts.PopulationSize ?? OptionsImmutable.PopulationSizeDefault;
        this.MaxGenerations = opts.MaxGenerations ?? OptionsImmutable.MaxGenerationsDefault;
        this.RepeatAmount = opts.RepeatAmount ?? fileOpts?.RepeatAmount ?? OptionsImmutable.RepeatAmountDefault;
        this.CrossoverProbability = opts.CrossoverProbability ?? fileOpts?.CrossoverProbability ?? OptionsImmutable.CrossoverProbabilityDefault;

        // assign first, check after for simpler handling
        this.PopulationCombination = opts.PopulationCombination ?? fileOpts?.PopulationCombination ?? OptionsImmutable.PopulationCombinationDefault;
        if (!OptionsImmutable.PopulationCombinationOptions.Contains(this.PopulationCombination))
        {
            System.Console.Error.WriteLine($"Unknown population combination: {this.PopulationCombination}");
            System.Environment.Exit(1);
        }

        // TODO: mutations

        this.PercentageToChange = opts.PercentageToChange ?? fileOpts?.PercentageToChange ?? OptionsImmutable.PercentageToChangeDefault;
        this.TerminalNodesProbability = opts.TerminalNodesProbability ?? fileOpts?.TerminalNodesProbability ?? OptionsImmutable.TerminalNodesProbabilityDefault;
        this.ValueNodeProbability = opts.ValueNodeProbability ?? fileOpts?.ValueNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SumNodeProbability = opts.SumNodeProbability ?? fileOpts?.SumNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ProductNodeProbability = opts.ProductNodeProbability ?? fileOpts?.ProductNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SinNodeProbability = opts.SinNodeProbability ?? fileOpts?.SinNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.PowerNodeProbability = opts.PowerNodeProbability ?? fileOpts?.PowerNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.UnaryMinusNodeProbability = opts.UnaryMinusNodeProbability ?? fileOpts?.UnaryMinusNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SigmoidNodeProbability = opts.SigmoidNodeProbability ?? fileOpts?.SigmoidNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ReLUNodeProbability = opts.ReLUNodeProbability ?? fileOpts?.ReLUNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ConditionNodeProbability = opts.ConditionNodeProbability ?? fileOpts?.ConditionNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        // exclude input node prob. since CartesianGP does not use it
    }

    public static OptionsImmutable From(Options opts)
    => new OptionsImmutable(
        opts,
        opts.JsonFilePath == null
            ? null
            : JsonSerializer.Deserialize<OptionsImmutable>(
                File.ReadAllText(opts.JsonFilePath, Encoding.ASCII)
            )
        );
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions(){
            WriteIndented = true,
            IgnoreReadOnlyFields = false
        });
    }
}