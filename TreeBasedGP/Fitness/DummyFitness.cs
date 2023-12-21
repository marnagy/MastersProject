public class DummyFitness : Fitness<TreeChromosome>
{
    public override double ComputeFitness(TreeChromosome ind)
        => 0;
}