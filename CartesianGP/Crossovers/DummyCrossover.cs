public class DummyCrossover : Crossover<CartesianChromosome>
{
    public override Tuple<CartesianChromosome, CartesianChromosome> Cross(CartesianChromosome ind1, CartesianChromosome ind2)
        => new Tuple<CartesianChromosome, CartesianChromosome>(
            ind1.Clone(),
            ind2.Clone()
        );
}