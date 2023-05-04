public class RandomFavoredSelection : Selection<CartesianChromosome>
{
    private Random _rng = new Random();
    public override Tuple<CartesianChromosome, CartesianChromosome> ChooseParents(IList<CartesianChromosome> population)
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