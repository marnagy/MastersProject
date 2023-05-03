public abstract class CartesianNode
{
    private static ParentIndices[]? _invalidParents;
    public static ParentIndices[] GetEmptyParents()
    {
        if (_invalidParents is null){
            _invalidParents = new[] { ParentIndices.GetInvalid(), ParentIndices.GetInvalid() };
        }
        return _invalidParents;
    }
    /// <summary>
    /// Store result inside instance
    /// </summary>
    abstract public void Compute(CartesianChromosome chromosome);

    public abstract CartesianNode Clone();
    public abstract CartesianNode Clone(ParentIndices[] newParents);

    public ParentIndices[] Parents;
    public double Result { get; protected set; }

    protected CartesianNode(ParentIndices[] parents) {
        if (parents.Length != 2)
            throw new ArgumentException($"Expected 2 parents' indices, got {parents.Length}.");

        this.Parents = parents;
    }
}