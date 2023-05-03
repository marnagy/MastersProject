public sealed class ReadOnly2DArray
{
    public readonly double[,] Inputs;
    public ReadOnly2DArray(double[,] inputs)
    {
        this.Inputs = inputs;
    }
}