public class DummyCombinedCrossover : Crossover<CombinedTreeChromosome>
{
    public override Tuple<CombinedTreeChromosome, CombinedTreeChromosome> Cross(CombinedTreeChromosome ind1, CombinedTreeChromosome ind2)
        => new Tuple<CombinedTreeChromosome, CombinedTreeChromosome>(
            ind1,
            ind2
        );
}