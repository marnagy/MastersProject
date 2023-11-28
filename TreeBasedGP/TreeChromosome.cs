public class TreeChromosome : Chromosome<TreeChromosome>
{
    public readonly TreeNode _rootNode;
    public readonly int Depth;
    public TreeChromosome(TreeNode rootNode)
    {
        this._rootNode = rootNode;
    }
    public override TreeChromosome Clone()
    => new TreeChromosome(this._rootNode.Clone());

    public override TreeChromosome CreateNew()
    {
        throw new NotImplementedException();
    }

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}