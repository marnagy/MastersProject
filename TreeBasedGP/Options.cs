using CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Options
{
    // general
    [Option("multi-threaded", Default = false, HelpText = $"Run GP in multiple threads. Default: {OptionsImmutable.MultiThreadedDefaultString}")]
    public bool MultiThreaded { get; set; }
    [Option("json", HelpText = "Input JSON for easier loading of hyperparameters.")]
    public string? JsonFilePath { get; set; }
    [Option("train-csv", Required = true, HelpText = "Train CSV of input values. Assumes numbers in en-US style.")]
    public string? TrainCSVFilePath { get; set; }
    [Option("test-csv", HelpText = "Test CSV of input values. Assumes numbers in en-US style. If not provided, use train-csv file.")]
    public string? TestCSVFilePath { get; set; }
    [Option("csv-inputs-amount", Required = true, HelpText = "Amount of input columns in CSV file.")]
    public int CSVInputsAmount { get; set; }
    [Option("csv-delimiter", HelpText = $"Delimiter of input CSV files. Default: {OptionsImmutable.CSVDelimiterDefaultString}")]
    public char? CSVDelimiter { get; set; }
    [Option("min-threads", HelpText = $"Minimum amount of threads to be used by ThreadPool class. Default: {OptionsImmutable.MinThreadsDefaultString}")]
    public int? MinThreads { get; set; }
    [Option("max-threads", HelpText = $"Maximum amount of threads to be used by ThreadPool class. Default: {OptionsImmutable.MaxThreadsDefaultString}")]
    public int? MaxThreads { get; set; }
    [Option("population-size", HelpText = $"Size of population in each generation. Default: {OptionsImmutable.PopulationSizeDefaultString}")]
    public int? PopulationSize { get; set; }
    [Option("max-generations", HelpText = $"Maximum amount of generations to evolve. Default: {OptionsImmutable.MaxGenerationsDefaultString}")]
    public int? MaxGenerations { get; set; }
    [Option("repeat-amount", HelpText = $"Amount of times to repeat the training of GPs. Default: {OptionsImmutable.RepeatAmountDefaultString}")]
    public int? RepeatAmount { get; set; }
    [Option("crossover-probability", HelpText = $"Probability of any crossover taking action. Default: {OptionsImmutable.CrossoverProbabilityDefaultString}")]
    public double? CrossoverProbability { get; set; }
    [Option("population-combination", HelpText = $"Choose the Population combination. [elitism, take-new, combine] Default: {OptionsImmutable.PopulationCombinationDefault}")]
    public string? PopulationCombination { get; set; }

    // specific for TreeBasedGP
    [Option("depth", Default = 3, HelpText = $"Maximum depth of starting trees. Default: {OptionsImmutable.DefaultTreeDepthDefaultString}")]
    public int? DefaultTreeDepth { get; set; }
    [Option("change-node-mutation-probability", HelpText = $"Probability of using ChangeNodeMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? ChangeNodeMutationProbability { get; set; }
    [Option("shuffle-children-mutation-probability", HelpText = $"Probability of using ShuffleChildrenMutation class. Default: {OptionsImmutable.MutationProbabilityDefaultString}")]
    public double? ShuffleChildrenMutationProbability { get; set; }

    // node probabilities
    [Option("percentage-to-change", HelpText = $"How much of 1 individual should mutation change. Default: {OptionsImmutable.PercentageToChangeDefaultString}")]
    public double? PercentageToChange { get; set; }
    [Option("terminal-nodes-probability", HelpText = "Probability of choosing from terminal nodes instead of non-terminal nodes.")]
    public double? TerminalNodesProbability { get; set; }
    [Option("value-node-weight", HelpText = $"Probability of choosing Value Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ValueNodeProbability { get; set; }
    [Option("sum-node-weight", HelpText = $"Probability of choosing Sum Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SumNodeProbability { get; set; }
    [Option("prod-node-weight", HelpText = $"Probability of choosing Product Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ProductNodeProbability { get; set; }
    [Option("sin-node-weight", HelpText = $"Probability of choosing Sin Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SinNodeProbability { get; set; }
    [Option("pow-node-weight", HelpText = $"Probability of choosing Power Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? PowerNodeProbability { get; set; }
    [Option("unary-minus-node-weight", HelpText = $"Probability of choosing UnaryMinus Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? UnaryMinusNodeProbability { get; set; }
    [Option("sig-node-weight", HelpText = $"Probability of choosing Sigmoid Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? SigmoidNodeProbability { get; set; }
    [Option("relu-node-weight", HelpText = $"Probability of choosing ReLU Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ReLUNodeProbability { get; set; }
    [Option("cond-node-weight", HelpText = $"Probability of choosing Condition Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? ConditionNodeProbability { get; set; }
    [Option("input-node-weight", HelpText = $"Probability of choosing Input Node. Default: {OptionsImmutable.NodeProbabilityDefaultString}")]
    public double? InputNodeProbability { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}