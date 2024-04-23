using CommandLine;
using System.Text.Json;

// constant interpolated strings with non-string types are NOT yet supported in .NET 8
// so I use *DefaultString suffix from OptionsImmutable to show defaults on --help screen

// while library CommandLineParser supports default CLI arguments, since we want to support
// loading arguments from JSON, if we used default params we cannot recognize if user ommited argument
// or specified it -> defaults are applied in OptionsImmutable.From function
public class Options
{
    // general
    [Option("multi-threaded", Default = OptionsImmutable.MultiThreadedDefault, HelpText = $"Run GP in multiple threads. Default: {OptionsImmutable.MultiThreadedDefaultString}")]
    public bool MultiThreaded { get; set; }
    [Option("json", Default = null, HelpText = "Input JSON for easier loading of hyperparameters.")]
    public string? JsonFilePath { get; set; } = null;
    [Option("train-csv", Required = true, HelpText = "Train CSV of input values. Assumes numbers in en-US style.")]
    public string TrainCSVFilePath { get; set; }
    [Option("test-csv", HelpText = "Test CSV of input values. Assumes numbers in en-US style. If not provided, use train-csv file.")]
    public string? TestCSVFilePath { get; set; }
    [Option("csv-inputs-amount", Required = true, HelpText = "Amount of input columns in CSV file.")]
    public int CSVInputsAmount { get; set; }
    [Option("csv-delimiter", HelpText = $"Delimiter of train and test CSV files. Default: {OptionsImmutable.CSVDelimiterDefaultString}")]
    public char CSVDelimiter { get; set; }
    [Option("min-threads", HelpText = $"Minimum amount of threads to be used by ThreadPool class. Default: {OptionsImmutable.MinThreadsDefaultString}")]
    public int? MinThreads { get; set; }
    [Option("max-threads", HelpText = $"Maximum amount of threads to be used by ThreadPool class. Default: {OptionsImmutable.MaxThreadsDefaultString}")]
    public int? MaxThreads { get; set; }
    // [Option("seed", HelpText = "Seed for the random number generator used in the GP algorithm.")]
    // public int? Seed { get; set; } = null;
    [Option("population-size", HelpText = $"Size of population in each generation. Default: {OptionsImmutable.PopulationSizeDefaultString}")]
    public int? PopulationSize { get; set; }
    [Option("max-generations", HelpText = $"Maximum amount of generations to evolve. Default: {OptionsImmutable.MaxGenerationsDefaultString}")]
    public int? MaxGenerations { get; set; }
    // [Option("mutation-probability", Default = 0.3d, HelpText = "Probability of each mutation taking action.")]
    // public double? MutationProbability { get; set; }
    [Option("repeat-amount", HelpText = $"Amount of times to repeat the training of GPs. Default: {OptionsImmutable.RepeatAmountDefaultString}")]
    public int? RepeatAmount { get; set; }
    [Option("crossover-probability", HelpText = $"Probability of each mutation taking action. Default: {OptionsImmutable.CrossoverProbabilityDefaultString}")]
    public double? CrossoverProbability { get; set; }

    // specific for CartesianGP
    // cannot print default using $-string due to being an array
    [Option("layer-sizes", HelpText = "Sizes of internal layer excluding inputs (left) and outputs (right) layer. Default: \"50, 50\"")]
    public IReadOnlyList<int>? LayerSizes { get; set; }
    [Option("population-combination", HelpText = $"Choose the Population combination. [elitism, take-new, combine] Default: {OptionsImmutable.PopulationCombinationDefault}")]
    public string? PopulationCombination { get; set; }

    // mutation probabilitites
    [Option("change-node-mutation-probability", HelpText = $"Probability of using ChangeNodeMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? ChangeNodeMutationProbability { get; set; }
    [Option("change-parents-mutation-probability", HelpText = $"Probability of using ChangeParentsMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? ChangeParentsMutationProbability { get; set; }
    [Option("add-node-to-layer-mutation-probability", HelpText = $"Probability of using AddNodeToLayerMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? AddNodeToLayerMutationProbability { get; set; }
    [Option("add-layer-mutation-probability", HelpText = $"Probability of using AddLayerMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? AddLayerMutationProbability { get; set; }
    [Option("remove-node-from-layer-mutation-probability", HelpText = $"Probability of using RemoveNodeFromLayerMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? RemoveNodeFromLayerMutationProbability { get; set; }
    [Option("remove-layer-mutation-probability", HelpText = $"Probability of using RemoveLayerMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? RemoveLayerMutationProbability { get; set; }

    // node probabilities
    [Option("percentage-to-change", HelpText = $"How much of 1 individual should mutation change. Default: {OptionsImmutable.PercentageToChangeDefaultString}")]
    public double? PercentageToChange { get; set; }
    [Option("terminal-nodes-probability", HelpText = $"Probability of choosing from terminal nodes instead of non-terminal nodes. Default: {OptionsImmutable.TerminalNodesProbabilityDefaultString}")]
    public double? TerminalNodesProbability { get; set; }
    [Option("value-node-weight", HelpText = $"Weight of choosing a value node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ValueNodeProbability { get; set; }
    [Option("sum-node-weight", HelpText = $"Weight of choosing a sum node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SumNodeProbability { get; set; }
    [Option("prod-node-weight", HelpText = $"Weight of choosing a product node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ProductNodeProbability { get; set; }
    [Option("sin-node-weight", HelpText = $"Weight of choosing a sin node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SinNodeProbability { get; set; }
    [Option("pow-node-weight", HelpText = $"Weight of choosing a power node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? PowerNodeProbability { get; set; }
    [Option("unary-minus-node-weight", HelpText = $"Weight of choosing a unary minus node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? UnaryMinusNodeProbability { get; set; }
    [Option("sig-node-weight", HelpText = $"Weight of choosing a sigmoid node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SigmoidNodeProbability { get; set; }
    [Option("relu-node-weight", HelpText = $"Weight of choosing a ReLU node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ReLUNodeProbability { get; set; }
    [Option("cond-node-weight", HelpText = $"Weight of choosing a condition node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ConditionNodeProbability { get; set; }
    // [Option("input-node-weight", HelpText = $"Weight of choosing a input node. Used for getting probability of choosing a node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    // public double? InputNodeProbability { get; set; }
    public void SetDefaults()
    {
        this.MultiThreaded = this.MultiThreaded == null ? this.MultiThreaded : false;
    }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}