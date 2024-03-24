using System.Globalization;

public class AccuracyFitness : Fitness<CartesianChromosome>
{
    private readonly double[,] Inputs;
    private readonly int[,] Outputs;
    private readonly int MaxThreads;
    public AccuracyFitness(double[,] inputs, int[,] outputs, int maxThreads)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.MaxThreads = maxThreads;
    }
    // public static AccuracyFitness Use(string csvFilePath, int inputSize, char delimiter = ',', bool ignoreColumnNames = true)
    // {
    //     if (csvFilePath is null)
    //         throw new ArgumentNullException(nameof(csvFilePath));

    //     List<double[]> inputs = new List<double[]>();
    //     List<double[]> outputs = new List<double[]>();

    //     foreach (var line in File.ReadLines(csvFilePath))
    //     {
    //         if (ignoreColumnNames)
    //         {
    //             ignoreColumnNames = false;
    //             continue;
    //         }
    //         var lineParts = line
    //             .Split(delimiter)
    //                 .Select(x => double.Parse(x, CultureInfo.InvariantCulture))
    //                 .ToArray();

    //         if (lineParts.Length <= inputSize)
    //             throw new ArgumentException($"A line does not consist of more than ${inputSize} (${nameof(inputSize)}) numbers. At least one output is required.");

    //         inputs.Add(
    //             lineParts
    //                 .Take(inputSize)
    //                 .ToArray()
    //         );
    //         outputs.Add(
    //             lineParts
    //                 .Skip(inputSize)
    //                 .ToArray()
    //         );
    //     }

    //     return new AccuracyFitness(inputs, outputs);
    // }
    public override double ComputeFitness(CartesianChromosome ind)
    {
        int correctAmount = 0;
        int rowsAmount = this.Inputs.GetRowsAmount();
        for (int row = 0; row < rowsAmount; row++)
        {
            // System.Console.Write($"Row: {row} || ");
            var computedResult = this.GetAsOneHot(
                ind.ComputeResult(this.Inputs.GetRow(row).ToArray())
            );
            var wantedResult = this.Outputs.GetRow(row).ToArray();

            if (wantedResult == computedResult)
                correctAmount += 1;
        }

        return 1 - ((double)correctAmount / rowsAmount);
    }
    public override void ComputeFitnessPopulation(CartesianChromosome[] population)
    {
        double[] accuracies = population.AsParallel()
            .WithDegreeOfParallelism(this.MaxThreads)
            .Select(ind => this.ComputeFitness(ind))
            .ToArray();
        
        for (int i = 0; i < population.Length; i++)
        {
            population[i].Fitness = accuracies[i];
        }
    }
    private int[] GetAsOneHot(IList<double> values)
    {
        var maxVal = values.Max();
        var maxIndex = Enumerable.Range(0, values.Count)
            .First(index => values[index] == maxVal);
        return Enumerable.Range(0, values.Count)
            .Select(index => index == maxIndex ? 1 : 0)
            .ToArray();
    }
}