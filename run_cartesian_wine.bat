Windows_x64\CartesianGP.exe --train-csv prepared_train_wine_sklearn.csv --test-csv prepared_test_wine_sklearn.csv --csv-inputs-amount 13 --csv-delimiter , --population-size 50 --max-generations 1000 --percentage-to-change 0.1 --crossover-probability 0.4 --terminal-nodes-probability 0.1 --change-node-mutation-probability 0.3 --change-parents-mutation-probability 0.2 --min-threads 2 --max-threads 4 --layer-sizes 50 50 100 100
