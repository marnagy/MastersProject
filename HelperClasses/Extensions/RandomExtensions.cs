public static class RandomExtensions
{
    /// <summary>
    /// Chooses one element from given list of elements.
    /// If no weights are provided, uses uniform probability.
    /// Weights will get normalized.
    /// </summary>
    /// <param name="arr">List of elements to choose from.</param>
    /// <param name="weights">
    /// Non-negative weight for each of the elements in previous argument.
    /// Weights will be normalized.
    /// </param>
    /// <returns>Chosen element</returns>
    /// <exception cref="ArgumentException"></exception>
    public static T Choose<T>(this Random rng, IReadOnlyList<T> arr, IReadOnlyList<double>? weights = null)
    {
        if (weights is null)
        {
            // use uniform probabilities
            weights = Enumerable.Range(0, arr.Count)
                .Select(_ => 1d / arr.Count)
                .ToArray();
        }

        if (arr.Count != weights.Count)
            throw new ArgumentException("Array of individuals and weights have to have the same length.");

        if (weights.Any(p => p < 0))
            throw new ArgumentException("All weights have to be non-negative numbers.");

        double randValue;
        randValue = rng.NextDouble();

        // normalize
        var probsSum = weights.Sum();
        var probs = weights
            .Select(p => p / probsSum)
            .ToArray();

        for (int i = 0; i < probs.Length - 1; i++)
        {
            if (randValue < probs[i])
                return arr[i];
            else
                randValue = randValue - probs[i];
        }

        // return the last one
        return arr[^1];
    }
}