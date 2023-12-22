public class AccuracyFitness : Fitness<TreeChromosome>
{
    private readonly double[,] Inputs;
    private int InputsAmount => this.Inputs.GetRowsAmount();
    private readonly double[,] Outputs;
    private readonly int OutputIndex;
    private readonly InputNode[] InputNodes;
    private readonly double Tolerance;
    public AccuracyFitness(double[,] inputs, double[,] outputs, int outputIndex, InputNode[] inputNodes, double tolerance = 0.1)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.OutputIndex = outputIndex;
        this.InputNodes = inputNodes;
        this.Tolerance = tolerance;
    }
    public override double ComputeFitness(TreeChromosome ind)
    {
        double totalDiff = 0d;
        for (int rowIndex = 0; rowIndex < this.Inputs.GetRowsAmount(); rowIndex++)
        {
            // set input nodes to values from row
            // fitness is calculated in single thread sequentially - so don't fear changing InputNodes
            IEnumerable<double> rowValues = this.Inputs.GetRow(rowIndex);
            foreach (
                (InputNode node, double newValue)
                    in Enumerable.Zip(this.InputNodes, this.Inputs.GetRow(rowIndex))
                )
            {
                // TODO: check
                node.Update(newValue);
            }

            var computedResult = ind.ComputeResult();
            var wantedResult = this.Outputs[rowIndex, this.OutputIndex];

            totalDiff += Math.Abs(wantedResult - computedResult);
        }

        return totalDiff;
    }
}