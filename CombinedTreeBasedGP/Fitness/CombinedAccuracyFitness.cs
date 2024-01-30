using System.Runtime.InteropServices;

public class CombinedAccuracyFitness : Fitness<CombinedTreeChromosome>
{
    private readonly double[,] Inputs;
    private readonly int[,] Outputs;
    private bool UseClip = true;
    private readonly InputFunctionality[] InputNodes;
    public CombinedAccuracyFitness(double[,] inputs, int[,] outputs, InputFunctionality[] inputNodes)
    {
        this.Inputs = inputs;
        this.Outputs = outputs;
        this.InputNodes = inputNodes;
    }
    private double MagicNormalizationCoefficient(CombinedTreeChromosome ind)
    => 1d/
    Math.Sqrt(
        ind.Subchromosomes
            .Select(subchrom => subchrom.GetDepth())
            .Average()
    );
    
    // Math.Pow(
    //     2,
    //     ind.Subchromosomes
    //         .Select(subchrom => subchrom.GetDepth())
    //         .Average()
    //     );
    public override double ComputeFitness(CombinedTreeChromosome ind)
    {
        // don't compute fitness again
        // if (ind.Fitness != CombinedTreeChromosome.DefaultFitness)
        //     return ind.Fitness;

        int accurateCounter = 0;
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
                accurateCounter += 1;
        }

        if (!this.HasInputNode(ind))
            return double.PositiveInfinity;

        int inputNodesAmount = this.CountInputNodes(ind);
        // without cast to double, the operation would mean "div"
        double accuracy = (double)accurateCounter / rowsAmount;
        // System.Console.Error.WriteLine($"Accuracy: {accuracy}");
        return 
            (1 - accuracy);
            //this.MagicNormalizationCoefficient(ind)
            //1d / inputNodesAmount;
    }

    public override void ComputeFitnessPopulation(CombinedTreeChromosome[] population)
    {
        int totalRows = this.Inputs.GetRowsAmount();
        double[] accurateCounters = new double[population.Length];
        for (int i = 0; i < totalRows; i++)
        {
            // if (i % 100 == 0)
            //     System.Console.Error.Write($"Calculating fitness: {i}/{totalRows}\r");
 
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
                // don't compute fitness again
                //.Where(tup => tup.ind.Fitness == TreeChromosome.DefaultFitness)
                .AsParallel()
                .Select(tup => (tup.index, computedResult: tup.ind.ComputeResults()))
                //.AsSequential()
                .ForEach(tup => {
                    var wantedResults = this.Outputs.GetRow(i);
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
                population[j].Fitness = accurateCounters[j]; // / (inputNodesAmount * population[j].Sub ); // * this.MagicNormalizationCoefficient(population[j]) / inputNodesAmount;
        }

        if (population.Select(ind => ind.Fitness).Any(fitness => fitness > 1d))
        {
            int a = 5;
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