from argparse import ArgumentParser, Namespace
import matplotlib.pyplot as plt
import pandas as pd
import os


def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument('-d', '--directory', required=True)

    return parser.parse_args()

def main():
    args = get_args()

    files = [
        os.path.join(args.directory, file)
        for file in os.listdir(args.directory)
        if file.endswith('.csv')
    ]
    fig, axes = plt.subplots(1, len(files), sharex=True, sharey=True)
    for index, file in enumerate(files):
        df = pd.read_csv(file, index_col='gen')
        axes[index].plot(df.index, df['minFitness'])
        # axes[index].plot(df.index, df['averageFitness'])

    # plt.yscale('log')
    plt.show()

if __name__ == '__main__':
    main()
