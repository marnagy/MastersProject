class TreeChromosome : Chromosome<TreeChromosome>
{
    private readonly TreeNode _rootNode;
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