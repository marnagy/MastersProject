public class GeneticAlgorithm<T> where T: Chromosome<T>
{
    /// <summary>
    /// Function to use when creating new random solution.
    /// </summary>
    private Func<T> createNewInd;
    private Mutation<T>[] mutations;
    private Crossover<T>[] crossovers;
    private Fitness<T> fitnessFunction;
    private Selection<T> selectionStrategy;
    /// <summary>
    /// Used for combining population and next_population at the end of every generation.
    /// This function is called **before** callback.
    /// </summary>
    private PopulationCombinationStrategy<T> populationStrategy;
    /// <summary>
    /// Function called after every generation.
    /// Can be used for collecting metadata for analysis after run.
    /// </summary>
    private Action<int, IReadOnlyList<T>> callback;
    public double CrossoverProbability;
    public double MutationProbability;
    public int MaxGenerations = 5_000;
    public int PopulationSize = 50;
    public int MinThreads = 2;
    public int MaxThreads = 4;
    public GeneticAlgorithm(Func<T> createNewFunc,
        IEnumerable<Mutation<T>> mutations,
        Crossover<T>[] crossovers, Fitness<T> fitness, Selection<T> selection,
        PopulationCombinationStrategy<T> popCombination, Action<int, IReadOnlyList<T>> callback)
    {
        this.createNewInd = createNewFunc;
        this.mutations = mutations.ToArray();
        this.crossovers = crossovers;
        this.fitnessFunction = fitness;
        this.populationStrategy = popCombination;
        this.selectionStrategy = selection;
        this.callback = callback;
    }
    
    public void Start()
    {
        ThreadPool.SetMinThreads((int)MinThreads, (int)MinThreads);
        ThreadPool.SetMaxThreads((int)MaxThreads, (int)MaxThreads);


        var population = Enumerable.Range(0, PopulationSize)
            .Select(_ => this.createNewInd() )
            .ToArray();

        for (int genNum = 0; genNum < MaxGenerations; genNum++)
        {
            // update Fitness
            population
                .ForEach(ind => ind.UpdateFitness(this.fitnessFunction)  );

            // select parents
            var parents = Enumerable.Range(0, population.Length / 2)
                .Select(_ => this.selectionStrategy.ChooseParents(population))
                .ToArray();

            // crossover
            // TODO: choose only 1 using GA.
            var next_population = parents
                .Select(p => (p, prob: Random.Shared.NextDouble()))
                .AsParallel()
                .Select(parents => {
                    if (parents.prob < this.CrossoverProbability)
                        return this.crossovers[0].Cross(parents.p.Item1, parents.p.Item2);
                    else
                        return parents.p;
                })
                .SelectMany(tup => new[] {tup.Item1, tup.Item2})
                .ToArray();

            // mutation
            foreach (var mut in this.mutations)
            {
                next_population = next_population
                    .AsParallel()
                    .Select(x => mut.Mutate(x, genNum) )
                    .ToArray();
            }

            // combine populations (elitism, ...)
            population = populationStrategy.Combine(population, next_population);

            this.callback(genNum, population);
        }
    }
}