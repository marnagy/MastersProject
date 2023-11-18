# General usage of the GPs

## Inputting parameters

### Command line interface
Using command line arguments (use *--help* with the run command to show the options) thanks to NuGeT package [CommandLineParser](https://github.com/commandlineparser/commandline)

### JSON
One of the possible command line arguments is *--json*. This signifies input JSON from which arguments can be loaded. This is a more convenient way of repeating the same input.

If you use input JSON **and** other commandline arguments that may conflict, the prefered argument will be loaded from *the command line argument*, **not from JSON**.

## Multithreading

This attribute can be set via command line argument *--use-multithreading* and uses additional flags *--min-threads* and *--max-threads*. These are used as setting in dotnet's ThreadPool class that is internally used by GA.

If you *don't* want to use multithreading, you can use flag *--single-threaded* for the algorithm to run in single-thread. This can be used in combination with flag *--seed* to reproduce results repeatedly.

# Cartesian GP

## Mutations

## Crossovers

# Tree-based GP

## Mutations

## Crossovers

# Dataset

We use Iris dataset taken from https://www.kaggle.com/datasets/uciml/iris