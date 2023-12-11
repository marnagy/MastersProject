using CommandLine;
using Microsoft.Win32.SafeHandles;

class Program
{
    public static void Main(string[] args)
    {
        Options cliArgs = Parser.Default.ParseArguments<Options>(args).Value;

        double terminalNodesProbability = cliArgs.TerminalNodesProbability;

        // prepare CSV
        (var inputs, var outputs) = PrepareCSV(
            cliArgs.CSVFilePath,
            cliArgs.CSVInputsAmount,
            cliArgs.CSVDelimiter
        );

        var inputNodes = Enumerable.Range(0, 2)
            .Select(i => new InputNode(1d + i, inputIndex: i))
            .ToArray();
        var terminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new ValueNode(0d, null), 0.3d},
            {inputNodes[0], 0.3d},
            {inputNodes[1], 0.3d},
        };
        var nonTerminalNodesProbabilities = new Dictionary<TreeNode, double> {
            {new SumNode(
                children: [inputNodes[1], inputNodes[0], new ValueNode(3d, null)]
                ), 0.4d}
        };
        int? seed = null;


        // how to handle setting inputs as terminals?
        // for each input, update terminalNodesProbabilities dictionary (remove old inputs, add new ones)
        TreeChromosome baseChromosome = new TreeChromosome(
            new SumNode(
                children: [inputNodes[1], inputNodes[0], new ValueNode(3d, null)]
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
    }
    public static (IReadOnlyList<double[]>, IReadOnlyList<double[]>) PrepareCSV(
            string CSVFilePath, int inputColumnsAmount,
            char delimiter
        )
    {
        var inputs = new List<double[]>();
        var outputs = new List<double[]>();
        using ( var sr = new StreamReader(File.OpenRead(CSVFilePath)))
        {
            string? line;
            int linePartsAmount = -1;
            IList<string> lineParts;
            bool columnNamesLine = true;
            while ( (line = sr.ReadLine()) != null)
            {
                if (columnNamesLine)
                {
                    linePartsAmount = line.Split(delimiter).Length;
                    columnNamesLine = false;
                    continue;
                }

                lineParts = line.Split(delimiter);
                if (lineParts.Count != linePartsAmount)
                    throw new Exception($"Invalid CSV: Expected amount of values {linePartsAmount}, actual {lineParts.Count}");
                
                inputs.Add(
                    Enumerable.Range(0, inputColumnsAmount)
                        .Select(i => double.TryParse(lineParts[i], out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
                outputs.Add(
                    Enumerable.Range(inputColumnsAmount, linePartsAmount - inputColumnsAmount)
                        .Select(i => double.TryParse(lineParts[i], out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
            }
        }

        return (inputs, outputs);
    }
}
