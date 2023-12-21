public class TakeNewCombination : PopulationCombinationStrategy<TreeChromosome>
{
    public override TreeChromosome[] Combine(TreeChromosome[] oldPopulation, TreeChromosome[] newPopulation)
        => newPopulation;
}