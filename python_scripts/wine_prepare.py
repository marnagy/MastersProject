import pandas as pd
import numpy as np
from sklearn.datasets import load_wine
import os


# preparing dataset from https://www.kaggle.com/datasets/oddrationale/mnist-in-csv
def main() -> None:
    wine = load_wine()
    df = pd.DataFrame(
        np.hstack((wine.data, wine.target.reshape(-1, 1))),
        dtype=np.float64
    )
    # add outputs

    df.to_csv(
        os.path.join(
            f'wine_sklearn.csv'
        ),
        index=False
    )

if __name__ == '__main__':
    main()