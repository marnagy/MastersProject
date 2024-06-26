using System.Runtime.InteropServices;

public class CombinedAccuracyFitness : Fitness<CombinedTreeChromosome>
{
    private readonly double[,] Inputs;
    private readonly int[,] Outputs;
    private const double coeff = 0.4;
    private bool UseClip = true;
    private readonly InputFunctionality[] InputNodes;
    private readonly int MaxThreads;
    public CombinedAccuracyFitness(double[,] inputs, int[,] outputs, InputFunctionality[] inputNodes, int maxThreads)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.InputNodes = inputNodes;
        this.MaxThreads = maxThreads;
    }
    private double MagicNormalizationCoefficient(CombinedTreeChromosome ind)
    => 1d + (
        ind.Subchromosomes
            .Select(subchrom => subchrom.GetDepth())
            .Average()
    )/1_000d;
    
    public override double ComputeFitness(CombinedTreeChromosome ind)
    {
        double accurateCounter = 0;
        int rowsAmount = this.Inputs.GetRowsAmount();
        for (int rowIndex = 0; rowIndex < rowsAmount; rowIndex++)
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
            
            IEnumerable<int> wantedResults = this.Outputs
                .GetRow(rowIndex);

            if (Enumerable
                    .Zip(this.ConvertToOnehot(values), wantedResults)
                    .All(tup => tup.First == tup.Second)
                )
                accurateCounter += 1d;
        }

        if (!this.HasInputNode(ind))
            return double.PositiveInfinity;

        double accuracy = accurateCounter / rowsAmount;
        ind.Score = 1d - accuracy;
        double fitness = ind.Score + CombinedAccuracyFitness.coeff*ind.GetDepth() / this.Inputs.GetRowsAmount();
        return fitness;
    }

    public override void ComputeFitnessPopulation(CombinedTreeChromosome[] population)
    {
        int totalRows = this.Inputs.GetRowsAmount();
        double[] accurateCounters = new double[population.Length];
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

            int[] wantedResults = this.Outputs.GetRow(i).ToArray();
            Enumerable.Range(0, population.Length)
                .Select(i => (index: i, ind: population[i]))
                .AsParallel().WithDegreeOfParallelism(this.MaxThreads)
                .Select(tup => (tup.index, computedResult: tup.ind.ComputeResults()))
                .ForEach(tup => {
                    double[] computedResults = tup.computedResult.ToArray();
                    var computedOnehot = this.ConvertToOnehot(computedResults);

                    if (Enumerable.Zip(computedOnehot, wantedResults).All(tup => tup.First == tup.Second))
                        accurateCounters[tup.index] += 1;
                });
        }

        for (int j = 0; j < population.Length; j++)
        {
            accurateCounters[j] = 1d - (accurateCounters[j] / totalRows);

            int inputNodesAmount = this.CountInputNodes(population[j]);
            if (inputNodesAmount == 0)
                population[j].Fitness = double.PositiveInfinity;
            else
            {
                population[j].Score = accurateCounters[j];
                population[j].Fitness = accurateCounters[j] + CombinedAccuracyFitness.coeff*population[j].GetDepth() / this.Inputs.GetRowsAmount();

            }
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
    private IEnumerable<int> ConvertToOnehot(double[] values)
    {
        int maxIndex = Enumerable.Range(0, values.Length)
            .Select(i => (index: i, value: values[i]))
            .MaxBy(tup => tup.value)
            .index;
        
        return Enumerable.Range(0, values.Length)
            .Select(i => i == maxIndex ? 1 : 0);
    }
}