start .\CombinedTreeBasedGP\bin\Release\net8.0\win-x64\CombinedTreeBasedGP.exe --multi-threaded --train-csv prepared_train_Iris.csv --csv-inputs-amount 4 --test-csv prepared_test_Iris.csv --population-size 100 --max-generations 50 --depth 6 --percentage-to-change 0.1 --crossover-probability 0.2 --mutation-probability 0.5 --terminal-nodes-probability 0.1 --min-threads 6 --max-threads 6 --repeat-amount 5