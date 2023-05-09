public class TakeNewCombination : PopulationCombinationStrategy<CartesianChromosome>
{
    public override CartesianChromosome[] Combine(CartesianChromosome[] oldPopulation, CartesianChromosome[] newPopulation)
        => newPopulation;
}