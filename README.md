# General usage of the GPs

## Inputting parameters

### Command line interface
Using command line arguments (use *--help* with the run command to show the options) thanks to NuGeT package [CommandLineParser](https://github.com/commandlineparser/commandline)

### JSON
One of the possible command line arguments is *--json*. This signifies input JSON from which arguments can be loaded. This is a more convenient way of repeating the same input.

If you use input JSON **and** other commandline arguments that may conflict, the prefered argument will be loaded from *the command line argument*, **not from JSON**.

## Multithreading

This attribute can be set via command line argument *--multi-threaded* and uses additional flags *--min-threads* and *--max-threads*. These are used as setting in dotnet's ThreadPool class that is internally used by GA.

If you *don't* want to use multithreading, you can use flag *--single-threaded* for the algorithm to run in single-thread. This can be used in combination with flag *--seed* to reproduce results repeatedly.

# Cartesian GP

<!-- ## Chromosome structure

Chromosome consists of layers of nodes where each can represent a value, a mathematical operation/function or branching condition (if ... then ... else ...). Each of these nodes can have upto 3 parents except input ($0^{th}$) layer which has invalid parents. Each of these parents have to be at least 1 layer closer to input compared to the child node.

Computation is done by layers from input layers to output layers. It is possible to compute results recursively from results, but this approach is not yet implemented. -->

## Mutations

- AddLayerMutation
- AddNodeToLayerMutation
- RemoveLayerMutation
- RemoveNodeFromLayerMutation
- ChangeNodeMutation
- ChangeParentsMutation

## Crossovers

- FixedIndexCrossover

# Combined Tree-based GP

## Mutations

- ChangeNodesMutation
- ShuffleNodesCombinedMutation

## Crossovers

- CombinedSwitchNodesCrossover

# Datasets

Used dataset were downloaded from *scikit-learn* library, submodule *datasets*.
