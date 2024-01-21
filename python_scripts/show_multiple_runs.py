from argparse import ArgumentParser, Namespace
import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd
import os
from glob import glob


def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument('-d', '--directory', required=True)

    return parser.parse_args()


def main():
    args = get_args()

    subdirs = list()
    for file_or_dir in os.listdir(args.directory):
        full_path = os.path.join(args.directory, file_or_dir)
        if os.path.isdir(full_path) and file_or_dir.startswith('run_'):
            subdirs.append(full_path)

    dfs = [
        pd.read_csv(
            os.path.join(subdir, file),
            # index_col='gen'
        )
        for subdir in subdirs
        for file in os.listdir(subdir)
        if file.endswith('.csv')
    ]
    df = pd.concat(dfs)

    print(df)

    # TODO: continue here
    print('Plotting fitness...', end='')
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='minFitness',
        color='r',
        label='MinFitness'
    )
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='averageFitness',
        color='g',
        ax=ax,
        label='AvgFitness'
    )

    print('Done')
    plt.yscale('log')
    plt.show()

    print('Plotting depth...', end='')
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='minDepth',
        color='r',
        label='MinDepth'
    )
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='averageDepth',
        color='g',
        ax=ax,
        label='AvgDepth'
    )

    print('Done')
    # plt.yscale('log')
    plt.show()


if __name__ == '__main__':
    main()
