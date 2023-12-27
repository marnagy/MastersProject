import pandas as pd
import os


# preparing dataset from https://www.kaggle.com/datasets/oddrationale/mnist-in-csv
def main() -> None:
    dir_name = 'MNIST'
    for file in os.listdir(dir_name):
        print(f'Processing {file} ...')
        path = os.path.join(dir_name, file)
        df = pd.read_csv(path, index_col=None)
        column_to_move = df.pop('label')
        df.insert(len(df.columns), 'label', column_to_move)
        df.to_csv(
            os.path.join(
                dir_name,
                f'updated_{file}'
            ),
            index=False
        )

if __name__ == '__main__':
    main()