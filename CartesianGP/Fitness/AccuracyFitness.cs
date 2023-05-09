using System.Globalization;

public class AccuracyFitness : Fitness<CartesianChromosome>
{
    private readonly IList<double[]> Inputs;
    private readonly IList<double[]> Outputs;
    private AccuracyFitness(IList<double[]> inputs, IList<double[]> outputs)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
    }
    public static AccuracyFitness Use(string csvFilePath, int inputSize, char delimiter = ',', bool ignoreColumnNames = true)
    {
        if (csvFilePath is null)
            throw new ArgumentNullException(nameof(csvFilePath));

        List<double[]> inputs = new List<double[]>();
        List<double[]> outputs = new List<double[]>();

        foreach (var line in File.ReadLines(csvFilePath))
        {
            if (ignoreColumnNames)
            {
                ignoreColumnNames = false;
                continue;
            }
            var lineParts = line
                .Split(delimiter)
                    .Select(x => double.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();

            if (lineParts.Length <= inputSize)
                throw new ArgumentException($"A line does not consist of more than ${inputSize} (${nameof(inputSize)}) numbers. At least one output is required.");

            inputs.Add(
                lineParts
                    .Take(inputSize)
                    .ToArray()
            );
            outputs.Add(
                lineParts
                    .Skip(inputSize)
                    .ToArray()
            );
        }

        return new AccuracyFitness(inputs, outputs);
    }
    public override double ComputeFitness(CartesianChromosome ind)
    {
        int correctAmount = 0;
        for (int row = 0; row < this.Inputs.Count; row++)
        {
            // System.Console.Write($"Row: {row} || ");
            var computedResult = ind.ComputeResult(this.Inputs[row]);
            var wantedResult = this.Outputs[row];

            if (wantedResult == computedResult)
                correctAmount += 1;
        }

        return correctAmount / this.Inputs.Count;
    }
}