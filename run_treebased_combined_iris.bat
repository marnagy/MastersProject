Windows_x64\CombinedTreeBasedGP.exe --train-csv prepared_train_iris_sklearn.csv --csv-inputs-amount 4 --test-csv prepared_test_iris_sklearn.csv --population-size 50 --max-generations 100 --depth 4 --percentage-to-change 0.15 --crossover-probability 0.2 --change-node-mutation-probability 0.3 --shuffle-children-mutation-probability 0.2 --terminal-nodes-probability 0.1 --min-threads 4 --max-threads 6 --population-combination combine