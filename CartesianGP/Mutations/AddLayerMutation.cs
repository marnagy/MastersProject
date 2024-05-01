public class AddLayerMutation : Mutation<CartesianChromosome>
{
    private const int _newLayerSize = 3;
    private readonly double TerminalNodesProbability;
    private readonly IReadOnlyDictionary<CartesianNode, double> TerminalNodesProbabilities;
    private IReadOnlyList<CartesianNode> TerminalNodes;
    private readonly IReadOnlyDictionary<CartesianNode, double> NonTerminalNodesProbabilities;
    private IReadOnlyList<CartesianNode> NonTerminalNodes;
    public AddLayerMutation(double probability,
        double terminalNodesProbability,
        IReadOnlyDictionary<CartesianNode, double> terminalNodesProbabilities,
        IReadOnlyDictionary<CartesianNode, double> nonTerminalNodesProbabilities): base(probability)
    {
        this.TerminalNodesProbability = terminalNodesProbability;
        this.TerminalNodesProbabilities = terminalNodesProbabilities;
        this.TerminalNodes = terminalNodesProbabilities.Keys.ToArray();
        this.NonTerminalNodesProbabilities = nonTerminalNodesProbabilities;
        this.NonTerminalNodes = nonTerminalNodesProbabilities.Keys.ToArray();
    }
    public override CartesianChromosome Mutate(CartesianChromosome ind, int genNum)
    {
        double rand_value;
        rand_value = Random.Shared.NextDouble();

        // don't mutate
        if (rand_value > this.MutationProbability)
            return ind.Clone();

        var layers = ind.DeepCopyLayers();

        int indexToInsertLayerTo;
        // output layer cannot change
        // if indexToInsertLayerTo == ^1 then output layer is moved by 1 index to the back
        indexToInsertLayerTo = Random.Shared.Next(layers.Count);

        var newLayer = Enumerable.Range(0, _newLayerSize)
            .Select(_ => {
                ParentIndices[] parents = CartesianChromosome.ChooseParents(
                    inputsAmount: ind.InputsAmount,
                    internalLayers: layers,
                    indexToInsertLayerTo
                );
                IReadOnlyList<CartesianNode> nodes;
                IReadOnlyList<double> nodeWeights;

                if (Random.Shared.NextDouble() < this.TerminalNodesProbability)
                {
                    nodes = this.TerminalNodes;
                    nodeWeights = this.TerminalNodes
                        .Select(node => this.TerminalNodesProbabilities[node])
                        .ToArray();
                }
                else
                {
                    nodes = this.NonTerminalNodes;
                    nodeWeights = this.NonTerminalNodes
                        .Select(node => this.NonTerminalNodesProbabilities[node])
                        .ToArray();
                }

                return Random.Shared.Choose(nodes, nodeWeights).Clone(parents);
            })
            .ToList();

        // TODO: re-index parents for nodes AFTER new layer that point to nodes AFTER new layer
        // !: new layer is NOT YET inserted
        foreach (var node in layers[indexToInsertLayerTo..].SelectMany(nodes => nodes))
        {
            ParentIndices[] nodeParents = node.Parents;
            ParentIndices[] fixedParents = nodeParents
                .Select(parent => parent.LayerIndex >= indexToInsertLayerTo
                    // shift LayerIndex further back (+1) 
                    ? new ParentIndices(){
                        LayerIndex=parent.LayerIndex+1,
                        Index=parent.Index
                    }
                    : parent.Clone())
                .ToArray();
        }
        
        layers.Insert(indexToInsertLayerTo, newLayer);

        var newChromosome = new CartesianChromosome(
            ind.InputsAmount,
            layers
        );

        if (!newChromosome.IsValid())
            throw new Exception("Created invalid choromosome in AddLayerMutation");

        return newChromosome;
    }
}