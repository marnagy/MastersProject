public class RandomFavoredSelection : Selection<CartesianChromosome>
{
    public RandomFavoredSelection(int? seed = null): base(seed) {}
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IReadOnlyList<CartesianChromosome> population)
    {   
        // we want to prioritize lower fitness
        // lower fitness -> higher probability
        var probs = population.Select(ind => 1/(ind.Fitness + 1)).ToArray();
        return new Tuple<CartesianChromosome, CartesianChromosome>(
            _rng.Choose(population, probs),
            _rng.Choose(population, probs)
        );
    }
}