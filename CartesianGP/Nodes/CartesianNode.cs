public abstract class CartesianNode
{
    static protected CartesianNode[] EmptyParents = new CartesianNode[0];
    /// <summary>
    /// Store result inside instance
    /// </summary>
    abstract public void Compute();

    public abstract CartesianNode Clone();

    public CartesianNode[] Parents;
    public double Result { get; protected set; }

    protected CartesianNode(CartesianNode[] parents) {
        this.Parents = parents;
    }
}