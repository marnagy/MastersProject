# installed
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split

# standard lib
from argparse import Namespace, ArgumentParser
from typing import List, Optional
import os


def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument(
        type=str, help='Filepath to input CSV file.', dest='filepath')
    parser.add_argument('-d', '--delimiter', default=',')
    parser.add_argument('-s', '--seed', type=int, help='Seed for train-test split function.')
    parser.add_argument(
        '--index-col', help='If CSV has index column, please include its name here.')
    parser.add_argument('--one-hot', action='store_true', default=False,
                        help='Convert all categorical columns to one-hot encoding,'
                        ' else convert to range 1-[number of categories]')
    parser.add_argument('--include-index', action='store_true', default=False,
                        help='Include index in final CSV file.')
    parser.add_argument('--train-ratio', type=float, help='If entered, the dataset will be split'
                        'with giver ratio of train-test data into separate files.')

    parser.description = 'Script assumes the last column is the output column.'

    return parser.parse_args()


def main() -> None:
    args = get_args()

    file_path, file_name = os.path.split(args.filepath)

    df = pd.read_csv(args.filepath, delimiter=args.delimiter, index_col=args.index_col)

    last_column_name = df.columns[-1]
    df[last_column_name] = df[last_column_name].apply(str)

    if args.train_ratio is not None:
        df_train, df_test = train_test_split(
            df,
            train_size=args.train_ratio,
            stratify=df[df.columns[-1]],
            random_state=args.seed    
        )
        dfs = [df_train, df_test]
        file_names = [f"prepared_train_{file_name}", f"prepared_test_{file_name}"]
    else:
        dfs = [df]
        file_names = [f"prepared_{file_name}"]

    info_already_printed = False
    for df, final_file_name in zip(dfs, file_names):
        column_index = 0
        last_column_converted_to_columns_amount = 1
        while column_index < len(df.columns):
            column_name = df.columns[column_index]
            if df.dtypes[column_name] == object:  # categorical
                if args.one_hot:
                    one_hot_encoded_column = pd.get_dummies(df[column_name])
                    df.drop(columns=[column_name], inplace=True)
                    for col_i, col in enumerate(one_hot_encoded_column.columns):
                        new_column_name = f'{column_name}-{col_i}'
                        df.insert(column_index + col_i, new_column_name,
                                one_hot_encoded_column[col])
                        df[new_column_name] = df[new_column_name].astype(np.int64)
                    column_index += len(one_hot_encoded_column.columns)
                    last_column_converted_to_columns_amount = len(
                        one_hot_encoded_column.columns)
                else:
                    values: List[str] = df[column_name].unique().tolist()
                    df.insert(column_index, f'{column_name}-converted',
                            df[column_name].apply(lambda val: values.index(val)))
                    df.drop(columns=[column_name], inplace=True)
                    column_index += 1
                    last_column_converted_to_columns_amount = 1
            else:
                column_index += 1
                last_column_converted_to_columns_amount = 1

        input_columns_amount = len(df.columns) - last_column_converted_to_columns_amount + int(args.include_index)

        if not info_already_printed:
            print("Example of output data:")
            print(df.head(5))
            print(f'Input columns: {input_columns_amount}, output_columns: {last_column_converted_to_columns_amount}')
            info_already_printed = True

        if args.train_ratio is not None:
            df.to_csv(os.path.join(
                    file_path,
                    final_file_name
                ),
                sep=args.delimiter,
                index=args.include_index
            )
            print(f'Generated file: {final_file_name}')
        else:
            df.to_csv(os.path.join(
                    file_path,
                    
                ),
                sep=args.delimiter,
                index=args.include_index
            )
            print(f'Generated file: {f"prepared_{file_name}"}')


if __name__ == '__main__':
    main()
