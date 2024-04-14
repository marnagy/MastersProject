using CommandLine;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(type: typeof(OptionsImmutable))]
public class OptionsImmutable
{
    // default values
    public const string? TestCSVFilePathDefault = null;
    public const int MinThreadsDefault = 2;
    public const int MaxThreadsDefault = 4;
    public const int PopulationSizeDefault = 50;
    public const int MaxGenerationsDefault = 500;
    public const int RepeatAmountDefault = 5;
    public const double CrossoverProbabilityDefault = 0.4d;
    // constant array not currently possible
    public static readonly IReadOnlyList<int> LayerSizesDefault = [50, 50];
    public const double MutationProbabilityDefault = 0d;
    // public const double ChangeNodeMutationProbabilityDefault = 0d;
    // public const double ChangeParentsMutationProbabilityDefault = 0d;
    // public const double AddNodeToLayerMutationProbabilityDefault = 0d;
    // public const double AddLayerMutationProbabilityDefault = 0d;
    // public const double RemoveNodeFromLayerMutationProbabilityDefault = 0d;
    // public const double RemoveLayerMutationProbabilityDefault = 0d;
    public const double PercentageToChangeDefault = 0.2d;
    public const double NodeProbabilityDefault = 0.2d;
    public const double TerminalNodesProbabilityDefault = 0.2d;
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

    // specific for CartesianGP
    public IReadOnlyList<int> LayerSizes { get; }
    // mutation probabilities
    public double ChangeNodeMutationProbability { get; }
    public double ChangeParentsMutationProbability { get; }
    public double AddNodeToLayerMutationProbability { get; }
    public double AddLayerMutationProbability { get; }
    public double RemoveNodeFromLayerMutationProbability { get; }
    public double RemoveLayerMutationProbability { get; }

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
        IReadOnlyList<int> layerSizes,
        double changeNodeMutationProbability,
        double changeParentsMutationProbability,
        double addNodeToLayerMutationProbability,
        double addLayerMutationProbability,
        double removeNodeFromLayerMutationProbability,
        double removeLayerMutationProbability,
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
        this.LayerSizes = layerSizes;

        this.ChangeNodeMutationProbability = changeNodeMutationProbability;
        this.ChangeParentsMutationProbability = changeParentsMutationProbability;
        this.AddNodeToLayerMutationProbability = addNodeToLayerMutationProbability;
        this.AddLayerMutationProbability = addLayerMutationProbability;
        this.RemoveNodeFromLayerMutationProbability = removeNodeFromLayerMutationProbability;
        this.RemoveLayerMutationProbability = removeLayerMutationProbability;

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
        this.MinThreads = fileOpts?.MinThreads ?? opts.MinThreads ?? OptionsImmutable.MinThreadsDefault;
        this.MaxThreads = fileOpts?.MaxThreads ?? opts.MaxThreads ?? OptionsImmutable.MaxThreadsDefault;
        this.PopulationSize = opts.PopulationSize ?? OptionsImmutable.PopulationSizeDefault;
        this.MaxGenerations = opts.MaxGenerations ?? OptionsImmutable.MaxGenerationsDefault;
        this.RepeatAmount = fileOpts?.RepeatAmount ?? opts.RepeatAmount ?? OptionsImmutable.RepeatAmountDefault;
        this.CrossoverProbability = fileOpts?.CrossoverProbability ?? opts.CrossoverProbability ?? OptionsImmutable.CrossoverProbabilityDefault;

        this.LayerSizes = fileOpts?.LayerSizes ?? opts.LayerSizes ?? OptionsImmutable.LayerSizesDefault;

        this.ChangeNodeMutationProbability = fileOpts?.ChangeNodeMutationProbability ?? opts.ChangeNodeMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;
        this.ChangeParentsMutationProbability = fileOpts?.ChangeParentsMutationProbability ?? opts.ChangeParentsMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;
        this.AddNodeToLayerMutationProbability = fileOpts?.AddNodeToLayerMutationProbability ?? opts.AddNodeToLayerMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;
        this.AddLayerMutationProbability = fileOpts?.AddLayerMutationProbability ?? opts.AddLayerMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;
        this.RemoveNodeFromLayerMutationProbability = fileOpts?.RemoveNodeFromLayerMutationProbability ?? opts.RemoveNodeFromLayerMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;
        this.RemoveLayerMutationProbability = fileOpts?.RemoveLayerMutationProbability ?? opts.RemoveLayerMutationProbability ?? OptionsImmutable.MutationProbabilityDefault;

        this.PercentageToChange = fileOpts?.PercentageToChange ?? opts.PercentageToChange ?? OptionsImmutable.PercentageToChangeDefault;
        this.TerminalNodesProbability = fileOpts?.TerminalNodesProbability ?? opts.TerminalNodesProbability ?? OptionsImmutable.TerminalNodesProbabilityDefault;
        this.ValueNodeProbability = fileOpts?.ValueNodeProbability ?? opts.ValueNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SumNodeProbability = fileOpts?.SumNodeProbability ?? opts.SumNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ProductNodeProbability = fileOpts?.ProductNodeProbability ?? opts.ProductNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SinNodeProbability = fileOpts?.SinNodeProbability ?? opts.SinNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.PowerNodeProbability = fileOpts?.PowerNodeProbability ?? opts.PowerNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.UnaryMinusNodeProbability = fileOpts?.UnaryMinusNodeProbability ?? opts.UnaryMinusNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.SigmoidNodeProbability = fileOpts?.SigmoidNodeProbability ?? opts.SigmoidNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ReLUNodeProbability = fileOpts?.ReLUNodeProbability ?? opts.ReLUNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
        this.ConditionNodeProbability = fileOpts?.ConditionNodeProbability ?? opts.ConditionNodeProbability ?? OptionsImmutable.NodeProbabilityDefault;
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