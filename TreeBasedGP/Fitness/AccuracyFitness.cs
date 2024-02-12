public class AccuracyFitness : Fitness<TreeChromosome>
{
    private readonly double[,] Inputs;
    private readonly int[,] Outputs;
    private readonly int OutputIndex;
    private bool UseClip = true;
    private readonly InputFunctionality[] InputNodes;
    private readonly int MaxThreads;
    public AccuracyFitness(double[,] inputs, int[,] outputs, int outputIndex, InputFunctionality[] inputNodes, int maxThreads)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.OutputIndex = outputIndex;
        this.InputNodes = inputNodes;
        this.MaxThreads = maxThreads;
    }
    private double MagicNormalizationCoefficient(TreeChromosome ind)
    => 1d/Math.Pow(2, ind.GetDepth());
    public override double ComputeFitness(TreeChromosome ind)
    {
        // don't compute fitness again
        if (ind.Fitness != TreeChromosome.DefaultFitness)
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
            
            if (this.UseClip)
            {
                if (wantedResult == 0 && computedResult < 0d)
                    computedResult = 0d;
                if (wantedResult == 1 && computedResult > 1d)
                    computedResult = 1d;
            }

            double diff = Math.Abs(wantedResult - computedResult);
            
            // make correct class more significant
            if (wantedResult == 1d && diff > 0)
                        diff *= this.Inputs.GetColumnsAmount();

            totalDiff += diff;
        }

        if (double.IsNaN(totalDiff) || !this.HasInputNode(ind))
            return double.PositiveInfinity;

        return totalDiff * this.MagicNormalizationCoefficient(ind) / this.CountInputNodes(ind);
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


            Enumerable.Range(0, population.Length)
                .Select(i => (index: i, ind: population[i]))
                // compute fitness only on newly created chromosomes
                .Where(tup => tup.ind.Fitness == TreeChromosome.DefaultFitness)
                .AsParallel().WithDegreeOfParallelism(this.MaxThreads)
                .Select(tup => (tup.index, computedResult: tup.ind.ComputeResult()))
                .ForEach(tup => {
                    int wantedResult = this.Outputs[i, this.OutputIndex];
                    double computedResult = tup.computedResult;

                    if (this.UseClip)
                    {
                        if (wantedResult == 0 && tup.computedResult < 0d)
                                computedResult = 0d;
                        if (wantedResult == 1 && tup.computedResult > 1d)
                            computedResult = 1d;
                    }

                    double diff = Math.Abs(wantedResult - computedResult);

                    if (wantedResult == 1 && diff > 0)
                        diff *= this.Inputs.GetColumnsAmount();

                    diffCounters[tup.index] += diff;
                });
        }

        // Enumerable.Range(0, population.Length)
        //     .Select(i => (i, ind: population[i]))
        //     .Where(tup => tup.ind.Fitness != TreeChromosome.DefaultFitness)
        //     .AsParallel()
        //     .ForEach(tup => {
        //         diffCounters[tup.i] = diffCounters[tup.i] / totalRows;

        //         if (double.IsNaN(diffCounters[tup.i]) || !this.HasInputNode(population[tup.i]))
        //             population[tup.i].Fitness = double.PositiveInfinity;
        //         else
        //             population[tup.i].Fitness = diffCounters[tup.i] * this.MagicNormalizationCoefficient(population[tup.i]) / this.CountInputNodes(population[tup.i]); //* this.MagicNormalizationCoefficient(population[j]);
        //     });

        for (int j = 0; j < population.Length; j++)
        {
            diffCounters[j] = diffCounters[j] / totalRows;

            if (double.IsNaN(diffCounters[j]) || !this.HasInputNode(population[j]))
                population[j].Fitness = double.PositiveInfinity;
            else
                population[j].Fitness = diffCounters[j] * this.MagicNormalizationCoefficient(population[j]) / this.CountInputNodes(population[j]); //* this.MagicNormalizationCoefficient(population[j]);
        }
    }
    private bool HasInputNode(TreeChromosome ind)
    => this.HasInputNode(ind.RootNode);
    private bool HasInputNode(TreeNodeMaster node)
    => node.Functionality is InputFunctionality
        || (
            node.Functionality.Arity > 0
            && node.Children[..node.Functionality.Arity]
                .Any(childNode => this.HasInputNode(childNode))
        );
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