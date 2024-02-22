/// <summary>
/// This Selections assumes only non-negative fitness for each individual.
/// </summary>
/// <typeparam name="T"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class RouletteWheelSelection<T> : Selection<T> where T: Chromosome<T>
{
    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population)
    {
        double[] fitnessValues = population.Select(ind => ind.Fitness).ToArray();

        return this.ChooseParents(
            population,
            fitnessValues
        );

        
    }

    public override Tuple<T, T> ChooseParents(IReadOnlyList<T> population, IReadOnlyList<double> probabilities)
    => new Tuple<T, T>(
            Random.Shared.ChooseProbs(population, probabilities),
            Random.Shared.ChooseProbs(population, probabilities)
        );
}