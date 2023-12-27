import pandas as pd
import numpy as np
from sklearn.datasets import load_digits
import os


# preparing dataset from https://www.kaggle.com/datasets/oddrationale/mnist-in-csv
def main() -> None:
    digits = load_digits()
    df = pd.DataFrame(
        np.hstack((digits.data, digits.target.reshape(-1, 1))),
        dtype=np.uint8
    )
    # add outputs

    df.to_csv(
        os.path.join(
            f'mnist_sklearn.csv'
        ),
        index=False
    )

if __name__ == '__main__':
    main()