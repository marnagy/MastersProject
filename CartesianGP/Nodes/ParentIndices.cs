public struct ParentIndices: IEquatable<ParentIndices>
{
    public static ParentIndices GetInvalid()
        => new ParentIndices(){
            LayerIndex = -1,
            Index = -1
        };

    public static bool operator ==(ParentIndices obj1, ParentIndices obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(ParentIndices obj1, ParentIndices obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(ParentIndices other)
    {
        return this.LayerIndex == other.LayerIndex
            && this.Index == other.Index;
    }

    public int LayerIndex { get; init; }
    public int Index { get; init; }
}