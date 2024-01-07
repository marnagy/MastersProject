using System.Text;

public struct TreeNodeMaster
{
    public TreeNodeMaster[]? Children;
    public bool HasChildren => this.Children != null;
    public const int ChildrenAmount = 3;
    public NodeFunctionality Functionality;
    public TreeNodeMaster(NodeFunctionality functionality, TreeNodeMaster[]? children)
    {
        this.Functionality = functionality;
        this.Children = children;
    }
    public void GetRepresentation(StringBuilder sb)
    {
        this.Functionality.GetRepresentation(sb, this.Children);
    }
    public double Compute()
    => this.Functionality.Compute(this.Children);
    public TreeNodeMaster Clone()
    => this.Clone(this.Children);
    public TreeNodeMaster Clone(TreeNodeMaster[]? children)
    => new TreeNodeMaster(this.Functionality, children);
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.Functionality);
        if (this.HasChildren)
        {
            sb.Append('[');
            sb.Append(string.Join<TreeNodeMaster>(", ", this.Children));
            sb.Append(']');
        }
        return sb.ToString();
    }
}