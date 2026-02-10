namespace SB.Common;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

public enum KnownSqlErrorType
{
    UknownError = 0,

    TimeoutError,
    UniqueKeyConstraintError,
    ReferenceConstraintError,
    CheckConstraintError,
    UniqueIndexError,
    TransactionDeadlockVictimError
}

public record KnownSqlError(
    KnownSqlErrorType Type,
    string ErrorMessage,
    string ConstraintOrIndexName,
    string ObjectName,
    string DuplicateKeyValue);

public static partial class KnownSqlErrorExtensions
{
    [GeneratedRegex("The timeout period elapsed prior to completion of the operation or the server is not responding")]
    public static partial Regex TimeoutErrorRegex();

    [GeneratedRegex("Violation of (?:PRIMARY|UNIQUE) KEY constraint '([a-zA-Z_0-9]+)'. Cannot insert duplicate key in object '([a-zA-Z_0-9]+\\.[a-zA-Z_0-9]+)'. The duplicate key value is (\\(.+\\))\\.")]
    public static partial Regex UniqueKeyConstraintErrorRegex();

    [GeneratedRegex("The (?:UPDATE|DELETE) statement conflicted with the REFERENCE constraint \"([a-zA-Z_0-9]+)\"\\.")]
    public static partial Regex ReferenceConstraintErrorRegex();

    [GeneratedRegex("The (?:INSERT|UPDATE) statement conflicted with the CHECK constraint \"([a-zA-Z_0-9]+)\"")]
    public static partial Regex CheckConstraintErrorRegex();

    [GeneratedRegex("Cannot insert duplicate key row in object '([a-zA-Z_0-9]+\\.[a-zA-Z_0-9]+)' with unique index '([a-zA-Z_0-9]+)'")]
    public static partial Regex UniqueIndexErrorRegex();

    [GeneratedRegex(@"Transaction \(Process ID [a-zA-Z_0-9]+\) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction")]
    public static partial Regex TransactionDeadlockVictimErrorRegex();

    private static Dictionary<KnownSqlErrorType, (int errorNumber, Regex pattern, int cn, int o, int dkv)> knownSqlErrors =
        new()
        {
            { KnownSqlErrorType.TimeoutError,
                (errorNumber: SqlServerErrorCodes.Timeout,
                pattern: TimeoutErrorRegex(),
                cn: 999,
                o: 999,
                dkv: 999)
            },
            { KnownSqlErrorType.UniqueKeyConstraintError,
                (errorNumber: SqlServerErrorCodes.ViolationOfUniqueKeyConstraint,
                pattern: UniqueKeyConstraintErrorRegex(),
                cn: 1,
                o: 2,
                dkv: 3)
            },
            { KnownSqlErrorType.ReferenceConstraintError,
                (errorNumber: SqlServerErrorCodes.ViolationOfConstraint,
                pattern: ReferenceConstraintErrorRegex(),
                cn: 1,
                o: 999,
                dkv: 999)
            },
            { KnownSqlErrorType.CheckConstraintError,
                (errorNumber: SqlServerErrorCodes.ViolationOfConstraint,
                pattern: CheckConstraintErrorRegex(),
                cn: 1,
                o: 999,
                dkv: 999)
            },
            { KnownSqlErrorType.UniqueIndexError,
                (errorNumber: SqlServerErrorCodes.ViolationOfUniqueIndex,
                pattern: UniqueIndexErrorRegex(),
                cn: 2,
                o: 1,
                dkv: 999)
            },
            { KnownSqlErrorType.TransactionDeadlockVictimError,
                (errorNumber: SqlServerErrorCodes.TransactionDeadlockVictim,
                pattern: TransactionDeadlockVictimErrorRegex(),
                cn: 999,
                o: 999,
                dkv: 999)
            },
        };

    public static KnownSqlError AsKnownSqlError(this SqlException ex)
    {
        foreach (var (type, (errorNumber, pattern, cn, o, dkv)) in knownSqlErrors)
        {
            if (ex.Number == errorNumber &&
                pattern.Match(ex.Message) is Match { Success: true } match)
            {
                return new KnownSqlError(
                    Type: type,
                    ErrorMessage: match.Value,
                    ConstraintOrIndexName: match.Groups[cn].Value,
                    ObjectName: match.Groups[o].Value,
                    DuplicateKeyValue: match.Groups[dkv].Value);
            }
        }

        return new KnownSqlError(
            Type: KnownSqlErrorType.UknownError,
            ErrorMessage: string.Empty,
            ConstraintOrIndexName: string.Empty,
            ObjectName: string.Empty,
            DuplicateKeyValue: string.Empty);
    }
}
