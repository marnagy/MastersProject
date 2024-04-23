import pandas as pd
import numpy as np
from sklearn.datasets import load_breast_cancer
import os


# preparing dataset from https://www.kaggle.com/datasets/oddrationale/mnist-in-csv
def main() -> None:
    breast_cancer = load_breast_cancer()
    df = pd.DataFrame(
        np.hstack((breast_cancer.data, breast_cancer.target.reshape(-1, 1))),
        dtype=np.float64
    )
    # add outputs

    df.to_csv(
        os.path.join(
            f'breast_cancer_sklearn.csv'
        ),
        index=False
    )

if __name__ == '__main__':
    main()