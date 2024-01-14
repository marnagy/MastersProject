FOR /L %%A IN (1,1,10) DO (
    CALL dotnet run --project ./TreeBasedGP -- --input-csv prepared_Iris.csv --input-csv-inputs-amount 4 --population-size 500 --max-generations 200 --depth 5 --percentage-to-change 0.2 --crossover-probability 0.6 --mutation-probability 0.3 --terminal-nodes-probability 0.01
)