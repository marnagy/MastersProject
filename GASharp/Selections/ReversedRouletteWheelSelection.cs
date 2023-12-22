/// <summary>
/// Like RouletteWheelSelection but prefers lower Fitness.
/// </summary>
/// <typeparam name="T"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReversedRouletteWheelSelection<T> : Selection<T> where T: Chromosome<T>
{
    public ReversedRouletteWheelSelection(int? seed = null): base(seed) {}
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    {
        double[] fitnessValues = population
            .Select(ind => 1/(1+ind.Fitness))
            .ToArray();

        return new Tuple<T, T>(
            this._rng.Choose(population, weights: fitnessValues),
            this._rng.Choose(population, weights: fitnessValues)
        );
    }
}