public class InputNode : ValueNode
{
    public readonly int InputIndex;
    public InputNode(double value, int inputIndex) : base(value)
    {
        this.InputIndex = inputIndex;
    }
    public void Update(double value)
    {
        this._value = value;
    }
    /// <summary>
    /// Creates new reference to input node.
    /// <b>Special behaviour for easier updates of input nodes.</b>
    /// </summary>
    /// <returns>New <b>reference</b> to the same object.</returns>
    public override TreeNode Clone()
    => this;
    /// <summary>
    /// Creates new reference to input node. <i>Ignores argument</i>
    /// <b>Special behaviour for easier updates of input nodes.</b>
    /// </summary>
    /// <returns>New <b>reference</b> to the same object.</returns>
    public override TreeNode Clone(TreeNode[]? children)
    => this.Clone();
    public override string ToString()
    => $"{this.GetType()}[Index:{this.InputIndex}, Value:{this._value}]";
}