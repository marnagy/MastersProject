public class RandomFavoredSelection : Selection<CartesianChromosome>
{
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IReadOnlyList<CartesianChromosome> population)
    {   
        // we want to prioritize lower fitness
        // lower fitness -> higher probability
        var probs = population.Select(ind => 1/(ind.Fitness + 1)).ToArray();
        return new Tuple<CartesianChromosome, CartesianChromosome>(
            Random.Shared.Choose(population, probs),
            Random.Shared.Choose(population, probs)
        );
    }
}