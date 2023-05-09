public class AccuracyFitness : Fitness<CartesianChromosome>
{
    private readonly ReadOnly2DArray Inputs;
    private readonly ReadOnly2DArray Outputs;
    private AccuracyFitness(ReadOnly2DArray inputs, ReadOnly2DArray outputs)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
    }
    public static AccuracyFitness Use(string csvFilePath, int inputSize, char delimiter = ',')
    {
        if (csvFilePath is null)
            throw new ArgumentNullException(nameof(csvFilePath));

        List<double[]> inputs = new List<double[]>();
        List<double[]> outputs = new List<double[]>();

        foreach (var line in File.ReadLines(csvFilePath))
        {
            var lineParts = line.Split(delimiter).Select(x => double.Parse(x)).ToArray();

            if (lineParts.Length <= delimiter)
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

        return new AccuracyFitness(
            new ReadOnly2DArray(inputs.ToArray()),
            new ReadOnly2DArray(outputs.ToArray())
        );
    }
    public override double ComputeFitness(CartesianChromosome ind)
    {
        // TODO: implement accuracy
        throw new NotImplementedException();
    }
}