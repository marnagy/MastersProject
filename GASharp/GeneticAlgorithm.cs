public class GeneticAlgorithm<T> where T: Chromosome<T>
{
    /// <summary>
    /// Function to use when creating new random solution.
    /// </summary>
    private Func<T> createNewInd;
    private IList<Mutation<T>> mutations;
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
    public double CrossoverProbability;
    public int MaxGenerations = 500;
    public int PopulationSize = 50;
    private bool UseTimes = false;
    // default is single-threaded
    public int MinThreads = 1;
    public int MaxThreads = 1;
    public GeneticAlgorithm(Func<T> createNewFunc,
        IList<Mutation<T>> mutations,
        Crossover<T>[] crossovers,
        Fitness<T> fitness,
        Selection<T> selection,
        PopulationCombinationStrategy<T> popCombination)
    {
        this.createNewInd = createNewFunc;
        this.mutations = mutations;
        this.crossovers = crossovers;
        this.fitnessFunction = fitness;
        this.populationStrategy = popCombination;
        this.selectionStrategy = selection;
    }
    
    public T[] Start(
        Action<int, IReadOnlyList<T>> callback,
        Func<IReadOnlyList<T>, bool> stopCondition)
    {
        if (MinThreads == 1 && MaxThreads == 1)
        {
            return this.StartSingleThreaded(callback, stopCondition);
        }

        ThreadPool.SetMinThreads(MinThreads, MinThreads);
        ThreadPool.SetMaxThreads(MaxThreads, MaxThreads);

        DateTime start;
        
        start = DateTime.UtcNow;

        var population = Enumerable.Range(0, PopulationSize)
            .AsParallel().WithDegreeOfParallelism(this.MaxThreads)
            .Select(_ => this.createNewInd())
            .ToArray();
        if (UseTimes)
            System.Console.Error.WriteLine($"Creating population took {DateTime.UtcNow - start} s");
        
        // update Fitness
        start = DateTime.UtcNow;
        this.fitnessFunction.ComputeFitnessPopulation(population);
        if (UseTimes)
            System.Console.Error.WriteLine($"Calculating fitness for initial population took {DateTime.UtcNow - start} s");

        T[] nextPopulation = new T[PopulationSize];
        for (int genNum = 0; genNum < MaxGenerations; genNum++)
        {
            {
                // select parents
                start = DateTime.UtcNow;
                double[] probs = RandomExtensions.CalculateProbabilities(
                    population,
                    population
                        .Select(ind => ind.Fitness)
                        .ToArray()
                );
                var parents = Enumerable.Range(0, population.Length / 2)
                    .AsParallel().AsUnordered().WithDegreeOfParallelism(this.MaxThreads)
                    .Select(_ => this.selectionStrategy.ChooseParents(population, probs))
                    .Select(tup => new Tuple<T,T>(tup.Item1.Clone(), tup.Item2.Clone()))
                    .ToArray();
                if (UseTimes)
                    System.Console.Error.WriteLine($"Selecting parents took {DateTime.UtcNow - start} s");

                // crossover
                // TODO: choose only 1 using GA.
                start = DateTime.UtcNow;
                Enumerable.Range(0, PopulationSize/2)
                    .Select(i => (index1: 2*i, index2: 2*i + 1, par: parents[i], prob: Random.Shared.NextDouble()))
                // parents
                //     .Select(p => (p, prob: Random.Shared.NextDouble()))
                    .AsParallel().AsUnordered().WithDegreeOfParallelism(this.MaxThreads)
                    .Select(tup => {
                        if (tup.prob < this.CrossoverProbability)
                            return (
                                index1: tup.index1,
                                index2: tup.index2,
                                par: this.crossovers[0].Cross(tup.par.Item1, tup.par.Item2)
                            );
                        else
                            return (
                                index1: tup.index1,
                                index2: tup.index2,
                                tup.par
                            );
                    })
                    .Select(tup => (
                        first: (
                            index: tup.index1,
                            par: tup.par.Item1
                        ),
                        second: (
                            index: tup.index2,
                            par: tup.par.Item2
                        )
                    ))
                    .ForAll(elem => {
                        nextPopulation[elem.first.index] = elem.first.par;
                        nextPopulation[elem.second.index] = elem.second.par;
                    });
                
                if (UseTimes)
                    System.Console.Error.WriteLine($"Crossover took {DateTime.UtcNow - start} s");
            }

            GC.Collect();

            // mutation
            start = DateTime.UtcNow;
            foreach (var mut in this.mutations)
            {
                Enumerable.Range(0, PopulationSize)
                    .AsParallel().WithDegreeOfParallelism(this.MaxThreads)
                    .Select(i => (index: i, ind: nextPopulation[i]))
                    .ForAll(tup => nextPopulation[tup.index] = mut.Mutate(tup.ind, genNum));
            }
            if (UseTimes)
                System.Console.Error.WriteLine($"Mutation took {DateTime.UtcNow - start} s");

            GC.Collect();

            // update Fitness
            this.fitnessFunction.ComputeFitnessPopulation(nextPopulation);

            // combine populations (elitism, ...)
            start = DateTime.UtcNow;
            population = populationStrategy.Combine(population, nextPopulation);
            if (UseTimes)
                System.Console.Error.WriteLine($"Population combination took {DateTime.UtcNow - start} s");

            // for (int i = 0; i < nextPopulation.Length; i++)
            // {
            //     nextPopulation[i] = null;
            // }

            start = DateTime.UtcNow;
            callback(genNum, population);
            if (UseTimes)
                System.Console.Error.WriteLine($"Callback took {DateTime.UtcNow - start} s");

            GC.Collect();

            if (stopCondition(population))
                break;
        }
        return population;
    }
    public T[] StartSingleThreaded(
        Action<int, IReadOnlyList<T>> callback,
        Func<IReadOnlyList<T>, bool> stopCondition)
    {
        var population = Enumerable.Range(0, PopulationSize)
            .Select(_ => this.createNewInd())
            .ToArray();
        
        // update Fitness
        population
            .ForEach(ind => ind.UpdateFitness(this.fitnessFunction));

        for (int genNum = 0; genNum < MaxGenerations; genNum++)
        {
            // select parents
            IList<Tuple<T,T>> parents = Enumerable.Range(0, population.Length / 2)
                .Select(_ => this.selectionStrategy.ChooseParents(population))
                .Select(par => new Tuple<T, T>(par.Item1.Clone(), par.Item2.Clone()))
                .ToArray();

            // crossover
            T[] nextPopulation = parents
                .Select(p => (p, prob: Random.Shared.NextDouble()))
                .Select(parents => {
                    if (parents.prob < this.CrossoverProbability)
                        return this.crossovers[0].Cross(parents.p.Item1, parents.p.Item2);
                    else
                        return new Tuple<T, T>(parents.p.Item1.Clone(), parents.p.Item2.Clone());
                })
                .SelectMany(tup => new[] {tup.Item1, tup.Item2})
                .ToArray();
            
            parents = null;

            // mutation
            foreach (var mut in this.mutations)
            {
                // nextPopulation
                //     .ForEach(x => mut.Mutate(x, genNum));
                nextPopulation = nextPopulation
                    .Select(x => mut.Mutate(x, genNum))
                    // .Select(ind => ind.Clone())
                    .ToArray();
            }

            // update Fitness
            foreach (var ind in nextPopulation)
            {
                ind.UpdateFitness(this.fitnessFunction);
            }

            // combine populations (elitism, ...)
            population = populationStrategy.Combine(population, nextPopulation);

            callback(genNum, population);

            if (stopCondition(population))
                break;
        }
        return population;
    }
}