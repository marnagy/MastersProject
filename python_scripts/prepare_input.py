from argparse import Namespace, ArgumentParser
import pandas as pd
import numpy as np
from typing import List


def get_args() -> Namespace:
    parser = ArgumentParser()

    parser.add_argument(
        type=str, help='Filepath to csv file.', dest='filepath')
    parser.add_argument('-d', '--delimiter', default=',')
    parser.add_argument(
        '--index-col', help='If CSV has index column, please include its name here.')
    parser.add_argument('--one-hot', action='store_true', default=False,
                        help='Convert all categorical columns to one-hot encoding,'
                        ' else convert to range 1-[number of categories]')
    parser.add_argument('--include-index', action='store_true', default=False,
                        help='Include index in final CSV file.')

    parser.description = 'Script assumes last column is the output column.'

    return parser.parse_args()


def main() -> None:
    args = get_args()
    print(args)

    df = pd.read_csv(args.filepath, delimiter=args.delimiter, index_col='Id')

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

    print(df.head(5))
    input_columns_amount = len(df.columns) - last_column_converted_to_columns_amount + int(args.include_index)
    print(f'Input columns: {input_columns_amount}, output_columns: {last_column_converted_to_columns_amount}')

    df.to_csv(f'prepared_{args.filepath}',
              sep=args.delimiter, index=args.include_index)


if __name__ == '__main__':
    main()
