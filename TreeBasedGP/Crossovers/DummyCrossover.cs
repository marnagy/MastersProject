public class DummyCrossover : Crossover<TreeChromosome>
{
    public override Tuple<TreeChromosome, TreeChromosome> Cross(TreeChromosome ind1, TreeChromosome ind2)
        => new Tuple<TreeChromosome, TreeChromosome>(
            ind1,
            ind2
        );
}