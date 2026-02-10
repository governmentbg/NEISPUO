namespace SB.ExtApi;

using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;
using SB.Common;

public class ExtApiSqlExceptionWhitelistFilter : SqlExceptionWhitelistFilter
{
    private static readonly (KnownSqlErrorType, int)[] ExtApiSqlExceptionWhitelist = new[]
    {
        (KnownSqlErrorType.TimeoutError, StatusCodes.Status508LoopDetected),
        (KnownSqlErrorType.CheckConstraintError, StatusCodes.Status400BadRequest),
        (KnownSqlErrorType.ReferenceConstraintError, StatusCodes.Status400BadRequest),
        (KnownSqlErrorType.UniqueKeyConstraintError, StatusCodes.Status409Conflict),
        (KnownSqlErrorType.UniqueIndexError, StatusCodes.Status409Conflict),
        (KnownSqlErrorType.TransactionDeadlockVictimError, StatusCodes.Status508LoopDetected)
    };

    public ExtApiSqlExceptionWhitelistFilter()
        : base(ExtApiSqlExceptionWhitelist, true)
    {
    }
}
