import pandas as pd
import numpy as np
from sklearn.datasets import load_iris
import os


# preparing dataset from https://www.kaggle.com/datasets/oddrationale/mnist-in-csv
def main() -> None:
    digits = load_iris()
    df = pd.DataFrame(
        np.hstack((digits.data, digits.target.reshape(-1, 1))),
        dtype=np.float64
    )
    # add outputs

    df.to_csv(
        os.path.join(
            f'iris_sklearn.csv'
        ),
        index=False
    )

if __name__ == '__main__':
    main()