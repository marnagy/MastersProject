using System.Globalization;

public static class CSVHelper
{
    public static (double[,], int[,]) PrepareCSV(
            string CSVFilePath, int inputColumnsAmount, char delimiter
        )
    {
        var inputs = new List<double[]>();
        var outputs = new List<int[]>();
        using ( var sr = new StreamReader(File.OpenRead(CSVFilePath)))
        {
            string? line;
            int linePartsAmount = -1;
            IList<string> lineParts;
            bool columnNamesLine = true;
            while ( (line = sr.ReadLine()) != null)
            {
                if (columnNamesLine)
                {
                    linePartsAmount = line.Split(delimiter).Length;
                    columnNamesLine = false;
                    continue;
                }

                lineParts = line.Split(delimiter);
                if (lineParts.Count != linePartsAmount)
                    throw new Exception($"Invalid CSV: Expected amount of values {linePartsAmount}, actual {lineParts.Count}");
                
                inputs.Add(
                    Enumerable.Range(0, inputColumnsAmount)
                        .Select(i => double.TryParse(lineParts[i], out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
                outputs.Add(
                    Enumerable.Range(inputColumnsAmount, linePartsAmount - inputColumnsAmount)
                        .Select(i => int.TryParse(lineParts[i], out int res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
            }
        }

        if (inputs.Count == 0)
            throw new Exception("No inputs found in CSV.");

        // convert to 2D array for space effieciency
        int inputsLength = inputs[0].Length;
        int outputsLength = outputs[0].Length;
        double[,] inputsArr = new double[inputs.Count, inputsLength];
        int[,] outputsArr = new int[outputs.Count, outputsLength];

        // ?: is there a more efficient way?
        for (int row_i = 0; row_i < inputs.Count; row_i++)
        {
            for (int col_i = 0; col_i < inputsLength; col_i++)
            {
                inputsArr[row_i, col_i] = inputs[row_i][col_i];
            }
            for (int col_i = 0; col_i < outputsLength; col_i++)
            {
                outputsArr[row_i, col_i] = outputs[row_i][col_i];
            }
        }

        return (inputsArr, outputsArr);
    }
}