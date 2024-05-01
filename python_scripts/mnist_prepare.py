import pandas as pd
import numpy as np
from sklearn.datasets import load_digits
import os


def main() -> None:
    digits = load_digits()
    df = pd.DataFrame(
        np.hstack((digits.data, digits.target.reshape(-1, 1))),
        dtype=np.uint8
    )

    df.to_csv(
        os.path.join(
            f'mnist_sklearn.csv'
        ),
        index=False
    )

if __name__ == '__main__':
    main()