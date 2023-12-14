using System.Globalization;

public static class CSVHelper
{
    public static (double[,], double[,]) PrepareCSV(
            string CSVFilePath, int inputColumnsAmount, char delimiter
        )
    {
        var inputs = new List<double[]>();
        var outputs = new List<double[]>();
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
                        .Select(i => double.TryParse(lineParts[i], CultureInfo.GetCultureInfo("en-US"), out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
                outputs.Add(
                    Enumerable.Range(inputColumnsAmount, linePartsAmount - inputColumnsAmount)
                        .Select(i => double.TryParse(lineParts[i], CultureInfo.GetCultureInfo("en-US"), out double res)
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
        double[,] outputsArr = new double[outputs.Count, outputsLength];

        // ?: is there a more efficient way?
        for (int i = 0; i < inputs.Count; i++)
        {
            for (int j = 0; j < inputsLength; j++)
            {
                inputsArr[i,j] = inputs[i][j];
            }
            for (int j = 0; j < outputsLength; j++)
            {
                outputsArr[i,j] = outputs[i][j];
            }
        }

        return (
            inputsArr,
            outputsArr
        );
    }
}