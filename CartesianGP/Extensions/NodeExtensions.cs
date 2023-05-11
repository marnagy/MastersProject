static class NodeExtensions
{
    public static T DeepCopy<T>(this T node) where T : CartesianNode
    {
        return (T)node.Clone();
    }
}