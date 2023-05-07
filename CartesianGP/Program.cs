var test_input = new[] {1,2,3,4,5};
var inputs = new double[5, 2];

var readOnlyInputs = new ReadOnly2DArray(inputs);

Dictionary<int, List<CartesianNode>> nodeCatalogue = new Dictionary<int, IList<CartesianNode>>
{
    {0, new List<CartesianNode> {new ValueNode(0d, CartesianNode.GetEmptyParents())} },
    {2, new List<CartesianNode> {new SumNode(CartesianNode.GetEmptyParents()),
    new ProductNode(CartesianNode.GetEmptyParents()) }
};

var layerSizes = new[] { 3, 10, 5, 2 };
var chromosome1 = CartesianChromosome.CreateNewRandom(
    layerSizes,
    nodeCatalogue
);
CartesianChromosome chromosome2 = chromosome1.Clone();

// compare chromosomes
System.Console.WriteLine($"Are chromosomes the same object? {Object.ReferenceEquals(chromosome1, chromosome2)}");

System.Console.WriteLine("Chromosome layers tests:");
for (int i = 0; i < layerSizes.Length; i++)
{
    System.Console.WriteLine($"Comparing layer {i}...");
    System.Console.WriteLine($"Does chromosome1 layer size correspond to the assignment? {chromosome1[i].Count == layerSizes[i]}");
    System.Console.WriteLine($"Does chromosome2 layer size correspond to the assignment? {chromosome2[i].Count == layerSizes[i]}");
    System.Console.WriteLine($"Are they the same object? {Object.ReferenceEquals(chromosome1[i].Count, chromosome2[i].Count)}");
    for (int j = 0; j < layerSizes[i]; j++)
    {
        System.Console.WriteLine($"Are the nodes the same object? {Object.ReferenceEquals(chromosome1[i][j], chromosome2[i][j])}");
        System.Console.WriteLine($"Are nodes the same? {chromosome1[i][j].Equals(chromosome2[i][j])}");
    }
}


// TODO: implement neccessary functions
var ga = new GeneticAlgorithm<CartesianChromosome>(
    () => CartesianChromosome.CreateNewRandom(
        new[] { 3, 10, 5, 2 },
        nodeCatalogue
    ),
    new[] {new ChangeNodeMutation(0.2, 0.5)},
    new[] {new DummyCrossover()},
    new DummyFitness(),
    new RandomFavoredSelection(),
    new DummyCombination(),
    (_,_) => { }
);

