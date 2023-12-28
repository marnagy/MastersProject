public class ConditionNode: TertiaryNode
{
    public ConditionNode(TreeNode[]? children): base(children)
    {
        ArgumentNullException.ThrowIfNull(children);
    }

    public override TreeNode Clone()
    => this.Clone(this.Children);

    public override TreeNode Clone(TreeNode[]? children)
    => new ConditionNode(children);

    public override double Compute()
    => this.Children[0].Compute() < 0
        ? this.Children[1].Compute()
        : this.Children[2].Compute();
    public override string ToString()
    => $"{this.GetType()}[Condition (<0): {this.Children[0]}, TrueBranch: {this.Children[1]}, FalseBranch: {this.Children[2]}]";

    public override string Representation()
    => $"if ({this.Children[0].Representation()} < 0) then {this.Children[1].Representation()} else {this.Children[2].Representation()}";
}