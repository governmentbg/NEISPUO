namespace SB.Common;

// The errors can be retrieved from SQL Server with the following query
//
// SELECT
//     message_id AS Error,
//     [text] AS [Description]
// FROM
//     sys.messages
// WHERE
//     language_id = 1033 and
//     message_id in (547, 1205, 2601, 2627, 13535)
// ORDER BY message_id;
//
// The timeout error code is generated from SqlClient and is not returned from SQL Server
public static class SqlServerErrorCodes
{
    public const int ViolationOfConstraint = 547; // The %ls statement conflicted with the %ls constraint "%.*ls". The conflict occurred in database "%.*ls", table "%.*ls"%ls%.*ls%ls.
    public const int ViolationOfUniqueIndex = 2601; // Cannot insert duplicate key row in object '%.*ls' with unique index '%.*ls'. The duplicate key value is %ls.
    public const int ViolationOfUniqueKeyConstraint = 2627; // Violation of %ls constraint '%.*ls'. Cannot insert duplicate key in object '%.*ls'. The duplicate key value is %ls.
    public const int DataModificationFailedOnSystemVersionedTable = 13535; // Data modification failed on system-versioned table '%.*ls' because transaction time was earlier than period start time for affected records.
    public const int TransactionDeadlockVictim = 1205; //Transaction (Process ID %d) was deadlocked on %.*ls resources with another process and has been chosen as the deadlock victim. Rerun the transaction.

    public const int Timeout = -2;
}
