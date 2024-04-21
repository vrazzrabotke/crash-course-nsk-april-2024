namespace Market.DAL;

internal record DbResult(DbResultStatus Status);

internal record DbResult<T>(T Result, DbResultStatus Status);