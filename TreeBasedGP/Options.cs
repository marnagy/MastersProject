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
    [Option("input-csv", Required = true, HelpText = "CSV of input values. Assumes numbers in en-US style.")]
    public string? CSVFilePath { get; set; }
    [Option("input-csv-inputs-amount", Required = true, HelpText = "Amount of input columns in CSV file.")]
    public int CSVInputsAmount { get; set; }
    [Option("input-csv-delimiter", Default = ',', HelpText = "Delimiter")]
    public char CSVDelimiter { get; set; }
    [Option("min-threads", Default = 2, /*Min = 1, Max = 32,*/ HelpText = "Minimum amount of threads to be used by ThreadPool class.")]
    public int MinThreads { get; set; }
    [Option("max-threads", Default = 4, /*Min = 1, Max = 32,*/ HelpText = "Maximum amount of threads to be used by ThreadPool class.")]
    public int MaxThreads { get; set; }
    [Option("seed", HelpText = "Seed for the random number generator used in the GP algorithm.")]
    public int? Seed { get; set; } = null;
    [Option("population-size", Default = 50, HelpText = "Size of population in each generation.")]
    public int PopulationSize { get; set; }
    [Option("max-generations", Default = 100, HelpText = "Maximum amount of generations to evolve.")]
    public int MaxGenerations { get; set; }
    [Option("mutation-probability", Default = 0.2d, HelpText = "Probability of each mutation taking action.")]
    public double MutationProbability { get; set; }
    [Option("repeat-amount", Default = 5, HelpText = "Amount of times to repeat the training of GPs.")]
    public int RepeatAmount { get; set; }

    // specific for TreeBasedGP
    [Option("terminal-nodes-probability", Default = 0.2d, HelpText = "Probability of choosing from terminal nodes instead of non-terminal nodes.")]
    public double TerminalNodesProbability { get; set; }
    [Option("depth", Default = 3, HelpText = "Maximum depth of starting trees.")]
    public int DefaultTreeDepth { get; set; }
    [Option("change-node-mutation-probability", Default = 0.2d, HelpText = "Probability of using ChangeNodeMutation class.")]
    public double ChangeNodeMutationProbability { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}