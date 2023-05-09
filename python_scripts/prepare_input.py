from argparse import Namespace, ArgumentParser
import pandas as pd
import numpy as np

def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument(type=str, help='Filepath to csv file.', dest='filepath')
    parser.add_argument('-d', '--delimiter', default=',')
    parser.add_argument('--one-hot-output', action='store_true', default=False)

    return parser.parse_args()

def main() -> None:
    args = get_args()
    print(args)
    
    df = pd.read_csv(args.filepath, delimiter=args.delimiter)

    if args.one_hot_output:
        one_hot_outputs = pd.get_dummies(df['Species'])
        for col in one_hot_outputs.columns:
            df.insert(len(df.columns), col, one_hot_outputs[col])
            df[col] = df[col].astype(np.float64)

        df.drop(['Id', 'Species'], axis='columns', inplace=True)
    else:
        # TODO: store class id in 'Species' column
        df.drop(['Id'], axis='columns', inplace=True)
    

    print(df)

    df.to_csv(f'prepared_{args.filepath}', sep=args.delimiter, index=False, )

if __name__ == '__main__':
    main()
