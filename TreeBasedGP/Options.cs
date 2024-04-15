using CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Options
{
    // general
    [Option("multi-threaded", Default = false, HelpText = "Run GP in multiple threads.")]
    public bool MultiThreaded { get; set; }
    [Option("json", Default = null, HelpText = "Input JSON for easier loading of hyperparameters.")]
    public string? JsonFilePath { get; set; }
    [Option("train-csv", Required = true, HelpText = "Train CSV of input values. Assumes numbers in en-US style.")]
    public string? TrainCSVFilePath { get; set; }
    [Option("test-csv", HelpText = "Test CSV of input values. Assumes numbers in en-US style. If not provided, use train-csv file.")]
    public string? TestCSVFilePath { get; set; }
    [Option("csv-inputs-amount", Required = true, HelpText = "Amount of input columns in CSV file.")]
    public int CSVInputsAmount { get; set; }
    [Option("csv-delimiter", Default = ',', HelpText = "Delimiter")]
    public char CSVDelimiter { get; set; }
    [Option("min-threads", /*Min = 1, Max = 32,*/ HelpText = "Minimum amount of threads to be used by ThreadPool class.")]
    public int? MinThreads { get; set; }
    [Option("max-threads", /*Min = 1, Max = 32,*/ HelpText = "Maximum amount of threads to be used by ThreadPool class.")]
    public int? MaxThreads { get; set; }
    [Option("seed", HelpText = "Seed for the random number generator used in the GP algorithm.")]
    public int? Seed { get; set; } = null;
    [Option("population-size", HelpText = "Size of population in each generation.")]
    public int? PopulationSize { get; set; }
    [Option("max-generations", HelpText = "Maximum amount of generations to evolve.")]
    public int? MaxGenerations { get; set; }
    [Option("repeat-amount", HelpText = "Amount of times to repeat the training of GPs.")]
    public int? RepeatAmount { get; set; }
    [Option("crossover-probability", Default = 0.5d, HelpText = "Probability of each mutation taking action.")]
    public double? CrossoverProbability { get; set; }

    // specific for TreeBasedGP
    [Option("terminal-nodes-probability", HelpText = "Probability of choosing from terminal nodes instead of non-terminal nodes.")]
    public double? TerminalNodesProbability { get; set; }
    [Option("depth", Default = 3, HelpText = "Maximum depth of starting trees.")]
    public int DefaultTreeDepth { get; set; }
    [Option("change-node-mutation-probability", HelpText = "Probability of using ChangeNodeMutation class.")]
    public double? ChangeNodeMutationProbability { get; set; }
    [Option("shuffle-children-mutation-probability", HelpText = "Probability of using ShuffleChildrenMutation class.")]
    public double? ShuffleChildrenMutationProbability { get; set; }
    [Option("population-combination", HelpText = $"Choose the Population combination. [elitism, take-new, combine] Default: {OptionsImmutable.PopulationCombinationDefault}")]
    public string? PopulationCombination { get; set; }

    // node probabilities
    [Option("percentage-to-change", HelpText = "How much of 1 individual should mutation change.")]
    public double? PercentageToChange { get; set; }
    [Option("value-node-weight")]
    public double? ValueNodeProbability { get; set; }
    [Option("sum-node-weight")]
    public double? SumNodeProbability { get; set; }
    [Option("prod-node-weight")]
    public double? ProductNodeProbability { get; set; }
    [Option("sin-node-weight")]
    public double? SinNodeProbability { get; set; }
    [Option("pow-node-weight")]
    public double? PowerNodeProbability { get; set; }
    [Option("unary-minus-node-weight")]
    public double? UnaryMinusNodeProbability { get; set; }
    [Option("sig-node-weight")]
    public double? SigmoidNodeProbability { get; set; }
    [Option("relu-node-weight")]
    public double? ReLUNodeProbability { get; set; }
    [Option("cond-node-weight")]
    public double? ConditionNodeProbability { get; set; }
    [Option("input-node-weight")]
    public double? InputNodeProbability { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}