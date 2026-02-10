namespace SB.Api;

using Microsoft.AspNetCore.Http;
using SB.ApiAbstractions;
using SB.Common;

public class ApiSqlExceptionWhitelistFilter : SqlExceptionWhitelistFilter
{
    private static readonly (KnownSqlErrorType, int)[] ApiSqlExceptionWhitelist = new[]
    {
        (KnownSqlErrorType.TimeoutError, StatusCodes.Status508LoopDetected),
        (KnownSqlErrorType.TransactionDeadlockVictimError, StatusCodes.Status508LoopDetected)
    };

    public ApiSqlExceptionWhitelistFilter()
        : base(ApiSqlExceptionWhitelist, false)
    {
    }
}
