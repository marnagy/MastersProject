public class DummySelection : Selection<TreeChromosome>
{
    public override Tuple<TreeChromosome, TreeChromosome> ChooseParents(IReadOnlyList<TreeChromosome> population)
        => new Tuple<TreeChromosome, TreeChromosome>(
            Random.Shared.Choose(population),
            Random.Shared.Choose(population)
        );
}