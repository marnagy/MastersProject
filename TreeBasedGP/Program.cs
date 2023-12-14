using System.Globalization;
using CommandLine;

class Program
{
    public static void Main(string[] args)
    {
        // set to en-us culture -> interpret real number with decimal point instead of decimal comma
        // from https://stackoverflow.com/questions/2234492/is-it-possible-to-set-the-cultureinfo-for-an-net-application-or-just-a-thread#comment32681459_2247570
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        Options cliArgs = Parser.Default.ParseArguments<Options>(args).Value;
        if (cliArgs == null)  // --help case
            return;

        if (!CheckArgs(cliArgs))
        {
            System.Console.Error.WriteLine("Invalid arguments.");
            System.Console.Error.WriteLine(cliArgs);
        }
        else
            System.Console.Error.WriteLine(cliArgs);

        double terminalNodesProbability = cliArgs.TerminalNodesProbability;

        // prepare CSV
        (var inputs, var outputs) = CSVHelper.PrepareCSV(
            cliArgs.CSVFilePath,
            cliArgs.CSVInputsAmount,
            cliArgs.CSVDelimiter
        );

        var inputNodes = Enumerable.Range(0, 2)
            .Select(i => new InputNode(1d + i, inputIndex: i))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new ValueNode(0d), 0.3d},
            {inputNodes[0], 0.3d},
            {inputNodes[1], 0.3d},
        };
        var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new SumNode(
                children: [inputNodes[1], inputNodes[0], new ValueNode(3d)]
                ), 0.4d}
        };
        int? seed = null;


        // how to handle setting inputs as terminals?
        // for each input, update terminalNodesProbabilities dictionary (remove old inputs, add new ones)
        TreeChromosome baseChromosome = new TreeChromosome(
            new SumNode(
                children: [inputNodes[1], inputNodes[0], new ValueNode(3d)]
                ),
            terminalNodesProbability,
            terminalNodesProbabilities,
            nonTerminalNodesProbabilities,
            seed
        );

        System.Console.WriteLine(baseChromosome);

        inputNodes[0].Update(5d);
        inputNodes[1].Update(20d);

        System.Console.WriteLine(baseChromosome);

        for (int row_i = 0; row_i < inputs.GetLength(0); row_i++)
        {
            System.Console.WriteLine(string.Join(", ", inputs.GetRow(row_i)));
        }
    }
    public static bool CheckArgs(Options args)
    {
        return args.CSVFilePath != null
            && args.CSVInputsAmount > 0;
    }
}
