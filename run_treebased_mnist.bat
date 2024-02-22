call dotnet run --project ./TreeBasedGP -- --multi-threaded --input-csv prepared_mnist_sklearn.csv --input-csv-inputs-amount 64 --population-size 50 --max-generations 100 --depth 10 --percentage-to-change 0.1 --crossover-probability 0.4 --mutation-probability 0.4 --terminal-nodes-probability 0.001 --min-threads 4 --max-threads 6