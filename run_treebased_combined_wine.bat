Windows_x64\CombinedTreeBasedGP.exe --train-csv prepared_train_wine_sklearn.csv --csv-inputs-amount 13 --test-csv prepared_test_wine_sklearn.csv --population-size 50 --max-generations 1000 --depth 5 --percentage-to-change 0.1 --crossover-probability 0.4 --change-node-mutation-probability 0.3 --shuffle-children-mutation-probability 0.3 --terminal-nodes-probability 0.1 --min-threads 2 --max-threads 4 --population-combination elitism