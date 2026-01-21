namespace NeispuoExtension.Exceptions.Database
{
    using System;

    using Messages = ExceptionMessages.Database;

    public class PostgreSqlException : DatabaseException
    {
        public PostgreSqlException()
            : base(Messages.MsSq)
        {
        }

        public PostgreSqlException(string message)
            : base(message)
        {
        }

        public PostgreSqlException(Exception innerException)
            : base(Messages.MsSq, innerException)
        {
        }

        public PostgreSqlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
