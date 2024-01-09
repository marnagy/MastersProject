public class AccuracyFitness : Fitness<TreeChromosome>
{
    private readonly double[,] Inputs;
    private int InputsAmount => this.Inputs.GetRowsAmount();
    private readonly int[,] Outputs;
    private readonly int OutputIndex;
    private readonly InputFunctionality[] InputNodes;
    public AccuracyFitness(double[,] inputs, int[,] outputs, int outputIndex, InputFunctionality[] inputNodes)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.OutputIndex = outputIndex;
        this.InputNodes = inputNodes;
    }
    public override double ComputeFitness(TreeChromosome ind)
    {
        // don't compute fitness again
        if (ind.Fitness != -1d)
            return ind.Fitness;

        double totalDiff = 0d;
        for (int rowIndex = 0; rowIndex < this.Inputs.GetRowsAmount(); rowIndex++)
        {
            // set input nodes to values from row
            // fitness is calculated in single thread sequentially - so don't fear changing InputNodes
            foreach (
                (InputFunctionality node, double newValue)
                    in Enumerable.Zip(this.InputNodes, this.Inputs.GetRow(rowIndex))
                )
            {
                node.Value = newValue;
            }

            double computedResult = ind.ComputeResult();
            // System.Console.Error.WriteLine($"Computed result: {computedResult}");
            int wantedResult = this.Outputs[rowIndex, this.OutputIndex];

            if (wantedResult == 0 && computedResult < 0d)
                computedResult = 0d;
            if (wantedResult == 1 && computedResult > 1d)
                computedResult = 1d;

            totalDiff += Math.Abs(wantedResult - computedResult);
        }

        if (double.IsNaN(totalDiff))
            totalDiff = double.PositiveInfinity;

        return totalDiff;
    }

    public override void ComputeFitnessPopulation(TreeChromosome[] population)
    {
        int totalRows = this.Inputs.GetRowsAmount();
        double[] diffCounters = new double[population.Length];
        for (int i = 0; i < totalRows; i++)
        {
            // update input nodes
            foreach (
                (InputFunctionality inputNode, double inputValue)
                    in Enumerable.Zip(this.InputNodes, this.Inputs.GetRow(i))
                )
            {
                inputNode.Value = inputValue;
            }

            int wantedResult = this.Outputs[i, this.OutputIndex];

            Enumerable.Range(0, population.Length)
                .Select(i => (index: i, ind: population[i]))
                // don't compute fitness again
                .Where(tup => tup.ind.Fitness != -1d)
                .AsParallel()
                .Select(tup => (tup.index, computedResult: tup.ind.ComputeResult()))
                .ForAll(tup => {
                    if (wantedResult == 0 && tup.computedResult < 0d)
                        tup.computedResult = 0d;
                    if (wantedResult == 1 && tup.computedResult > 1d)
                        tup.computedResult = 1d;

                    double diff = Math.Abs(wantedResult - tup.computedResult);

                    diffCounters[tup.index] += diff;
                });
        }

        for (int j = 0; j < population.Length; j++)
        {
            diffCounters[j] = diffCounters[j] / totalRows;

            if (double.IsNaN(diffCounters[j]))
                diffCounters[j] = double.PositiveInfinity;

            if (population[j].Fitness == -1d)
                continue;

            population[j].Fitness = diffCounters[j];
        }
    }
}