
public class CombinedSwitchNodesCrossover : Crossover<CombinedTreeChromosome>
{
    public override Tuple<CombinedTreeChromosome, CombinedTreeChromosome> Cross(CombinedTreeChromosome ind1, CombinedTreeChromosome ind2)
    {
        foreach ((var subchrom1, var subchrom2)
            in Enumerable.Zip(ind1.Subchromosomes, ind2.Subchromosomes))
        {
            List<TreeNodeMaster> nodes1 = new();
            List<TreeNodeMaster> nodes2 = new();

            // DFS to get all nodes that have children
            lock (this)
            {
                this.LoadNodes(subchrom1.RootNode, nodes1);
                this.LoadNodes(subchrom2.RootNode, nodes2);
            }

            // no nodes with children found
            if (nodes1.Count == 0 || nodes2.Count == 0)
                continue;

            var chosenParent1 = Random.Shared.Choose(nodes1);
            nodes1.Clear();
            var chosenParent2 = Random.Shared.Choose(nodes2);
            nodes2.Clear();

            // choose random child of each parent
            var childIndex1 = Random.Shared.Next(chosenParent1.Functionality.Arity);
            var childIndex2 = Random.Shared.Next(chosenParent2.Functionality.Arity);

            // !: this requires in-place changes
            var temp = chosenParent1.Children[childIndex1].Clone();
            chosenParent1.Children[childIndex1] = chosenParent2.Children[childIndex2].Clone();
            chosenParent2.Children[childIndex2] = temp;
        }

        return new Tuple<CombinedTreeChromosome, CombinedTreeChromosome>(
            ind1, ind2
        );
    }

    private void LoadNodes(TreeNodeMaster node, List<TreeNodeMaster> nodes)
    {
        if (node.Functionality.Arity == 0)
            return;

        nodes.Add(node);
        foreach (var childNode in node.Children[..node.Functionality.Arity])
        {
            this.LoadNodes(childNode, nodes);
        }
    }
}