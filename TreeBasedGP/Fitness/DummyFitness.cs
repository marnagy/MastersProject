public class DummyFitness : Fitness<TreeChromosome>
{
    public override double ComputeFitness(TreeChromosome ind)
        => 0;

    public override void ComputeFitnessPopulation(TreeChromosome[] population)
    {
        foreach (var ind in population)
        {
            ind.Fitness = 0;
        }
    }
}