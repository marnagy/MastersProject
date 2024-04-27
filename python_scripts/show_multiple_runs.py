from argparse import ArgumentParser, Namespace
import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd
import os
from glob import glob


def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument('-d', '--directory', required=True)
    parser.add_argument('-s', '--save', action='store_true', dest='save', help='If used, graph will be saved and not shown.')

    return parser.parse_args()


def main():
    args = get_args()

    subdirs = list()
    for file_or_dir in os.listdir(args.directory):
        full_path = os.path.join(args.directory, file_or_dir)
        if os.path.isdir(full_path) and file_or_dir.startswith('run_'):
            subdirs.append(full_path)

    print("Loading csv\'s...", end='')
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
    print('Done')

    os.chdir(args.directory)

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

    plt.yscale('log')
    plt.ylim(top=1)
    if not args.save:
        plt.show()
        print('Done')
    else:
        plt.tight_layout()
        plt.savefig('fitness_graph.png')
        print('Fitness figure has been saved.')
    plt.clf()
    
    print('Plotting score...', end='')
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='minScore',
        color='r',
        label='ScoreOfMin'
    )
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='averageScore',
        color='g',
        ax=ax,
        label='AvgScore'
    )

    plt.yscale('log')
    plt.ylim(top=1)
    if not args.save:
        plt.show()
        print('Done')
    else:
        plt.tight_layout()
        plt.savefig('score_graph.png')
        print('Score figure has been saved.')
    plt.clf()

    print('Plotting depth...', end='')
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='minDepth',
        color='r',
        label='DepthOfMin'
    )
    ax = sns.lineplot(
        data=df,
        x='gen',
        y='averageDepth',
        color='g',
        ax=ax,
        label='AvgDepth'
    )

    # plt.yscale('log')
    if not args.save:
        plt.show()
        print('Done')
    else:
        plt.tight_layout()
        plt.savefig('depth_graph.png')
        print('Depth figure has been saved.')


if __name__ == '__main__':
    main()
