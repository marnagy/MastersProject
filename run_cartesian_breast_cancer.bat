Windows_x64\CartesianGP.exe --multi-threaded --train-csv prepared_train_breast_cancer_sklearn.csv --test-csv prepared_test_breast_cancer_sklearn.csv --csv-inputs-amount 30 --csv-delimiter , --population-size 50 --max-generations 1000 --percentage-to-change 0.1 --crossover-probability 0.4  --terminal-nodes-probability 0.1 --change-node-mutation-probability 0.3 --change-parents-mutation-probability 0.2 --min-threads 4 --max-threads 8 --layer-sizes 100 100 100 100 50 50 50 --population-combination combine --add-node-to-layer-mutation-probability 0.1