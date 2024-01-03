
public class SwitchNodesCrossover : Crossover<TreeChromosome>
{
    public override Tuple<TreeChromosome, TreeChromosome> Cross(TreeChromosome ind1, TreeChromosome ind2)
    {
        List<TreeNode> nodes1 = new();
        List<TreeNode> nodes2 = new();

        // DFS to get all nodes that have children
        this.LoadNodes(ind1.RootNode, nodes1);
        var chosenParent1 = Random.Shared.Choose(nodes1);
        nodes1.Clear();
        this.LoadNodes(ind2.RootNode, nodes2);
        var chosenParent2 = Random.Shared.Choose(nodes1);
        nodes2.Clear();

        // choose random child of each parent
        var childIndex1 = Random.Shared.Next(TreeNode.ChildrenAmount);
        var childIndex2 = Random.Shared.Next(TreeNode.ChildrenAmount);

        // !: this requires in-place changes

        // TODO: finish crossover
    }

    private void LoadNodes(TreeNode node, List<TreeNode> nodes)
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