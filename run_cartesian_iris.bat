Windows_x64\CartesianGP.exe --multi-threaded --train-csv prepared_train_iris_sklearn.csv --csv-delimiter , --csv-inputs-amount 4 --test-csv prepared_test_iris_sklearn.csv --population-size 50 --max-generations 1000 --percentage-to-change 0.2 --crossover-probability 0.2 --terminal-nodes-probability 0.1 --min-threads 2 --max-threads 4 --layer-sizes 20 50 100 --change-node-mutation-probability 0.4 --change-parents-mutation-probability 0.3 --population-combination elitism --add-node-to-layer-mutation-probability 0.1