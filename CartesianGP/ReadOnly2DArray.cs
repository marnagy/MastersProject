public sealed class ReadOnly2DArray
{
    public readonly double[,] Inputs;
    public ReadOnly2DArray(double[,] inputs)
    {
        this.Inputs = inputs;
    }
    public ReadOnly2DArray(double[][] inputs)
    {
        double [,] localInputs = new double[inputs.Length, inputs[0].Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            for (int j = 0; j < inputs[i].Length; j++)
            {
                localInputs[i,j] = inputs[i][j];
            }
        }

        this.Inputs = localInputs;
    }
}