/// <summary>
/// This Selections assumes only non-negative fitness for each individual.
/// </summary>
/// <typeparam name="T"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class RouletteWheelSelection<T> : Selection<T> where T: Chromosome<T>
{
    public RouletteWheelSelection(int? seed = null): base(seed) {}
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    {
        double[] fitnessValues = population.Select(ind => ind.Fitness).ToArray();

        return new Tuple<T, T>(
            this._rng.Choose(population, weights: fitnessValues),
            this._rng.Choose(population, weights: fitnessValues)
        );
    }
}