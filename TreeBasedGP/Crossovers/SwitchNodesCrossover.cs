
public class SwitchNodesCrossover : Crossover<TreeChromosome>
{
    public override Tuple<TreeChromosome, TreeChromosome> Cross(TreeChromosome ind1, TreeChromosome ind2)
    {
        List<TreeNodeMaster> nodes1 = new();
        List<TreeNodeMaster> nodes2 = new();

        // DFS to get all nodes that have children
        this.LoadNodes(ind1.RootNode, nodes1);
        this.LoadNodes(ind2.RootNode, nodes2);

        // no nodes with children found
        if (nodes1.Count == 0 || nodes2.Count == 0)
            return new Tuple<TreeChromosome, TreeChromosome>(
                ind1,
                ind2
            );

        var chosenParent1 = Random.Shared.Choose(nodes1);
        nodes1.Clear();
        var chosenParent2 = Random.Shared.Choose(nodes2);
        nodes2.Clear();

        // choose random child of each parent
        var childIndex1 = Random.Shared.Next(TreeNodeMaster.ChildrenAmount);
        var childIndex2 = Random.Shared.Next(TreeNodeMaster.ChildrenAmount);

        // !: this requires in-place changes
        var temp = chosenParent1.Children[childIndex1];
        chosenParent1.Children[childIndex1] = chosenParent2.Children[childIndex2];
        chosenParent2.Children[childIndex2] = temp;

        return new Tuple<TreeChromosome, TreeChromosome>(
            ind1, ind2
        );
    }

    private void LoadNodes(TreeNodeMaster node, List<TreeNodeMaster> nodes)
    {
        if (!node.HasChildren)
            return;

        nodes.Add(node);
        foreach (var childNode in node.Children)
        {
            this.LoadNodes(childNode, nodes);
        }
    }
}