public class CombinedDifferenceFitness : Fitness<CombinedTreeChromosome>
{
    private readonly double[,] Inputs;
    private readonly int[,] Outputs;
    private bool UseClip = true;
    private readonly InputFunctionality[] InputNodes;
    public CombinedDifferenceFitness(double[,] inputs, int[,] outputs, InputFunctionality[] inputNodes)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.InputNodes = inputNodes;
    }
    private double MagicNormalizationCoefficient(CombinedTreeChromosome ind)
    => 1d;
    public override double ComputeFitness(CombinedTreeChromosome ind)
    {
        // don't compute fitness again
        if (ind.Fitness != CombinedTreeChromosome.DefaultFitness)
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

            double[] values = ind.Subchromosomes
                .Select(subchrom => subchrom.ComputeResult())
                .ToArray();            
            
            int[] wantedResults = this.Outputs
                .GetRow(rowIndex)
                .ToArray();

            double diff = 0;

            foreach ((double computedRes, int wantedResult)
                in Enumerable.Zip(values, wantedResults))
            {
                double computedResult = computedRes;
                if (this.UseClip)
                {
                    if (wantedResult == 0 && computedResult < 0d)
                        computedResult = 0d;
                    if (wantedResult == 1 && computedResult > 1d)
                        computedResult = 1d;
                }

                diff += Math.Abs(wantedResult - computedResult);
            }

            totalDiff += diff;
        }

        int inputNodesAmount = this.CountInputNodes(ind);
        if (double.IsNaN(totalDiff) || inputNodesAmount == 0)
            return double.PositiveInfinity;

        return totalDiff * this.MagicNormalizationCoefficient(ind); // / inputNodesAmount;
    }

    public override void ComputeFitnessPopulation(CombinedTreeChromosome[] population)
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


            Enumerable.Range(0, population.Length)
                .Select(i => (index: i, ind: population[i]))
                .Select(tup => (tup.index, computedResult: tup.ind.ComputeResults()))
                .ForEach(tup => {
                    IEnumerable<int> wantedResults = this.Outputs.GetRow(i);
                    IEnumerable<double> computedResults = tup.computedResult;

                    foreach ((double computedRes, int wantedResult)
                        in Enumerable.Zip(computedResults, wantedResults))
                    {
                        double computedResult = computedRes;
                        if (this.UseClip)
                        {
                            if (wantedResult == 0 && computedResult < 0d)
                                    computedResult = 0d;
                            if (wantedResult == 1 && computedResult > 1d)
                                computedResult = 1d;
                        }

                        double diff = Math.Abs(wantedResult - computedResult);

                        diffCounters[tup.index] += diff;
                    }
                });
        }

        for (int j = 0; j < population.Length; j++)
        {
            diffCounters[j] = diffCounters[j] / totalRows;

            int inputNodesAmount = this.CountInputNodes(population[j]);
            if (double.IsNaN(diffCounters[j]) || inputNodesAmount == 0)
                population[j].Fitness = double.PositiveInfinity;
            else
                population[j].Fitness = diffCounters[j] * this.MagicNormalizationCoefficient(population[j]);
        }
    }
    private bool HasInputNode(CombinedTreeChromosome ind)
    => ind.Subchromosomes
        .All(subchrom => this.HasInputNode(subchrom));
    private bool HasInputNode(TreeChromosome ind)
    => this.HasInputNode(ind.RootNode);
    private bool HasInputNode(TreeNodeMaster node)
    => node.Functionality is InputFunctionality
        || (
            node.Functionality.Arity > 0
            && node.Children[..node.Functionality.Arity]
                .Any(childNode => this.HasInputNode(childNode))
        );
    private int CountInputNodes(CombinedTreeChromosome ind)
    => ind.Subchromosomes.Sum(subchrom => this.CountInputNodes(subchrom));
    private int CountInputNodes(TreeChromosome ind)
    => this.CountInputNodes(ind.RootNode);
    private int CountInputNodes(TreeNodeMaster node)
    {
        if (node.Functionality is InputFunctionality)
            return 1;
        else
        {
            int counter = 0;
            if (node.Functionality.Arity > 0)
            {
                counter += node.Children[..node.Functionality.Arity]
                    .Select(this.CountInputNodes)
                    .Sum();
            }
            return counter;
        }
    }
}