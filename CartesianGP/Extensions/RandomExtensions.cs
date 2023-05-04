static class RandomExtensions
{
    public static T Choose<T>(this Random rng, IList<T> arr, IList<double> probabilities)
    {
        if ( arr.Count != probabilities.Count )
            throw new ArgumentException("Array of individuals and probabilities have to have the same length.");
        
        if ( probabilities.Any(p => p < 0) )
            throw new ArgumentException("All probabilities have to be non-negative numbers.");

        double randValue;
        lock(rng)
        {
            randValue = rng.NextDouble();
        }
        
        // normalize
        var probsSum = probabilities.Sum();
        var probs = probabilities
            .Select(p => p / probsSum)
            .ToArray();
        
        for (int i = 0; i < probabilities.Count - 1; i++)
        {
            if ( randValue < probabilities[i] )
                return arr[i];
            else
                randValue = randValue - probabilities[i];
        }

        // return the last one
        return arr[^1];
    }
}