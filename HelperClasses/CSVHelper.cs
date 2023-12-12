public static class CSVHelper
{
    public static (IReadOnlyList<double[]>, IReadOnlyList<double[]>) PrepareCSV(
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
                        .Select(i => double.TryParse(lineParts[i], out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
                outputs.Add(
                    Enumerable.Range(inputColumnsAmount, linePartsAmount - inputColumnsAmount)
                        .Select(i => double.TryParse(lineParts[i], out double res)
                            ? res
                            : throw new Exception($"Invalid CSV format: Expected double, found {lineParts[i]}") )
                        .ToArray()
                );
            }
        }

        return (inputs, outputs);
    }
}