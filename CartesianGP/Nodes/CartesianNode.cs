using System.Text;

public abstract class CartesianNode : IEquatable<CartesianNode>
{
    public const int ParentsAmount = 3;
    /// <summary>
    /// Arity (amount of parents) of the node. Value *-1* signifies not assigned.
    /// </summary>
    /// <value></value>
    public int Arity { get; protected init; } = -1;
    private static ParentIndices[]? _invalidParents;
    public static ParentIndices[] GetEmptyParents()
    {
        if (_invalidParents is null)
        {
            _invalidParents = Enumerable.Range(0, CartesianNode.ParentsAmount)
                .Select(_ => ParentIndices.GetInvalid())
                .ToArray();
        }
        return _invalidParents;
    }
    /// <summary>
    /// Store result inside instance
    /// </summary>
    abstract public double Compute(CartesianChromosome chromosome);

    public virtual CartesianNode Clone()
    => this.Clone(
        this.Parents
            .Select(x => x)
            .ToArray()
    );
    public abstract CartesianNode Clone(ParentIndices[] newParents);

    public abstract bool Equals(CartesianNode? other);
    /// <summary>
    /// Stores Indices for CartesianChromosome class.
    /// </summary>
    public ParentIndices[] Parents;
    public double Result { get; protected set; }

    protected CartesianNode(ParentIndices[] parents)
    {
        if (parents.Length != ParentsAmount)
            throw new ArgumentException($"Expected {ParentsAmount} parents' indices, got {parents.Length}.");

        this.Parents = parents;
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.GetType());
        sb.Append(':');
        if (this is not ValueNode)
        {
            sb.Append('[');
            sb.Append(string.Join(',', this.Parents));
            sb.Append(']');
        }
        return sb.ToString();
    }
}