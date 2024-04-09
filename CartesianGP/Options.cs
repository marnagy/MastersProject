using CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Options
{
    // general
    [Option("multi-threaded", Default = false, HelpText = "Run GP in multiple threads.")]
    public bool? MultiThreaded { get; set; }
    [Option("json", Default = null, HelpText = "Input JSON for easier loading of hyperparameters.")]
    public string? JsonFilePath { get; set; } = null;
    [Option("train-csv", Required = true, HelpText = "Train CSV of input values. Assumes numbers in en-US style.")]
    public string TrainCSVFilePath { get; set; }
    [Option("test-csv", HelpText = "Test CSV of input values. Assumes numbers in en-US style. If not provided, use train-csv file.")]
    public string? TestCSVFilePath { get; set; }
    [Option("csv-inputs-amount", Required = true, HelpText = "Amount of input columns in CSV file.")]
    public int CSVInputsAmount { get; set; }
    [Option("csv-delimiter", Required = true, Default = ',', HelpText = "Delimiter")]
    public char CSVDelimiter { get; set; }
    [Option("min-threads", HelpText = "Minimum amount of threads to be used by ThreadPool class.")]
    public int? MinThreads { get; set; }
    [Option("max-threads", HelpText = "Maximum amount of threads to be used by ThreadPool class.")]
    public int? MaxThreads { get; set; }
    // [Option("seed", HelpText = "Seed for the random number generator used in the GP algorithm.")]
    // public int? Seed { get; set; } = null;
    [Option("population-size", HelpText = "Size of population in each generation.")]
    public int? PopulationSize { get; set; }
    [Option("max-generations", HelpText = "Maximum amount of generations to evolve.")]
    public int? MaxGenerations { get; set; }
    // [Option("mutation-probability", Default = 0.3d, HelpText = "Probability of each mutation taking action.")]
    // public double? MutationProbability { get; set; }
    [Option("repeat-amount", HelpText = "Amount of times to repeat the training of GPs.")]
    public int? RepeatAmount { get; set; }
    [Option("crossover-probability", HelpText = "Probability of each mutation taking action.")]
    public double? CrossoverProbability { get; set; }

    // specific for CartesianGP
    [Option("layer-sizes", HelpText = "Sizes of internal layer excluding inputs (left) and outputs (right) layer.")]
    public IList<int>? LayerSizes { get; set; }

    // mutation probabilitites
    [Option("change-node-mutation-probability", HelpText = "Probability of using ChangeNodeMutation class.")]
    public double? ChangeNodeMutationProbability { get; set; }
    [Option("change-parents-mutation-probability", HelpText = "Probability of using ChangeParentsMutation class.")]
    public double? ChangeParentsMutationProbability { get; set; }
    [Option("add-node-to-layer-mutation-probability", HelpText = "Probability of using AddNodeToLayerMutation class.")]
    public double? AddNodeToLayerMutationProbability { get; set; }
    [Option("remove-node-from-layer-mutation-probability", HelpText = "Probability of using RemoveNodeFromLayerMutation class.")]
    public double? RemoveNodeFromLayerMutationProbability { get; set; }

    // node probabilities
    [Option("percentage-to-change", HelpText = "How much of 1 individual should mutation change.")]
    public double? PercentageToChange { get; set; }
    [Option("terminal-nodes-probability", HelpText = "Probability of choosing from terminal nodes instead of non-terminal nodes.")]
    public double? TerminalNodesProbability { get; set; }
    [Option("value-node-weight", HelpText = "Weight of choosing a value node. Used for getting probability of choosing a node.")]
    public double? ValueNodeProbability { get; set; }
    [Option("sum-node-weight", HelpText = "Weight of choosing a sum node. Used for getting probability of choosing a node.")]
    public double? SumNodeProbability { get; set; }
    [Option("prod-node-weight", HelpText = "Weight of choosing a product node. Used for getting probability of choosing a node.")]
    public double? ProductNodeProbability { get; set; }
    [Option("sin-node-weight", HelpText = "Weight of choosing a sin node. Used for getting probability of choosing a node.")]
    public double? SinNodeProbability { get; set; }
    [Option("pow-node-weight", HelpText = "Weight of choosing a power node. Used for getting probability of choosing a node.")]
    public double? PowerNodeProbability { get; set; }
    [Option("unary-minus-node-weight", HelpText = "Weight of choosing a unary minus node. Used for getting probability of choosing a node.")]
    public double? UnaryMinusNodeProbability { get; set; }
    [Option("sig-node-weight", HelpText = "Weight of choosing a sigmoid node. Used for getting probability of choosing a node.")]
    public double? SigmoidNodeProbability { get; set; }
    [Option("relu-node-weight", HelpText = "Weight of choosing a ReLU node. Used for getting probability of choosing a node.")]
    public double? ReLUNodeProbability { get; set; }
    [Option("cond-node-weight", HelpText = "Weight of choosing a condition node. Used for getting probability of choosing a node.")]
    public double? ConditionNodeProbability { get; set; }
    [Option("input-node-weight", HelpText = "Weight of choosing a input node. Used for getting probability of choosing a node.")]
    public double? InputNodeProbability { get; set; }
    public void SetDefaults()
    {
        this.MultiThreaded = this.MultiThreaded == null ? this.MultiThreaded : false;
    }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}