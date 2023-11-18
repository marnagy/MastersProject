using CommandLine;

public class Options
{
    [Option("single-threaded", Default = true, Required = false, HelpText = "Run GP in single-thread.")]
    public bool SingleThreaded { get; set; }
    [Option("multi-threaded", Default = false, Required = false, HelpText = "Run GP in multiple threads.")]
    public bool MultiThreaded { get; set; }
    [Option("json", Default = null, HelpText = "Input JSON for easier loading of hyperparameters.")]
    public string? JsonFilePath { get; set; }
    [Option("min-threads", Default = 2, Min = 1, Max = 32, HelpText = "Minimum amount of threads to be used by ThreadPool class.")]
    public int MinThreads { get; set; }
    [Option("max-threads", Default = 4, Min = 1, Max = 32, HelpText = "Maximum amount of threads to be used by ThreadPool class.")]
    public int MaxThreads { get; set; }
    [Option("seed", HelpText = "Seed for the random number generator used in the GP algorithm.")]
    public int Seed { get; set; }
}   