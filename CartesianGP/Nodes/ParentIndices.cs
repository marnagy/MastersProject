public struct ParentIndices
{
    public static ParentIndices GetInvalid()
        => new ParentIndices(){
            LayerIndex = -1,
            Index = -1
        };
    public int LayerIndex { get; init; }
    public int Index { get; init; }
}