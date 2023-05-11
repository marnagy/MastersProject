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

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        
        if (obj is ParentIndices parent)
        {
            return this.LayerIndex == parent.LayerIndex
                && this.Index == parent.Index;
        }
        else 
            return false;
    }
    public override int GetHashCode()
    {
        return $"{this.LayerIndex}:{this.Index}".GetHashCode();
    }
}