namespace SB.Domain;

using Microsoft.Data.SqlClient;

public class DomainUpdateSqlException : DomainException
{
    public DomainUpdateSqlException(SqlException sqlException)
        : base(sqlException.Message)
    {
        this.SqlException = sqlException;
    }

    public SqlException SqlException { get; init; }
}
