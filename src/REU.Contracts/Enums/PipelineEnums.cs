namespace Contracts.Enums;

public enum Timeframe { m1, m5, m15, m30, h1, h4, D, W, M, ANY, IRREGULAR }

public enum LoaderType { Sqlite, Csv, Parquet }

public enum FuserType { LastObservationCarriedForward }

public enum WriterType { CsvFile, ParquetFile, JsonFile, Console, None }
