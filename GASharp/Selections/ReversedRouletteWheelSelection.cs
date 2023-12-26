/// <summary>
/// Like RouletteWheelSelection but prefers lower Fitness.
/// </summary>
/// <typeparam name="T"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReversedRouletteWheelSelection<T> : Selection<T> where T: Chromosome<T>
{
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    {
        double[] fitnessValues = population
            .Select(ind => 1/(1+ind.Fitness))
            .ToArray();

        return new Tuple<T, T>(
            Random.Shared.Choose(population, weights: fitnessValues),
            Random.Shared.Choose(population, weights: fitnessValues)
        );
    }
}