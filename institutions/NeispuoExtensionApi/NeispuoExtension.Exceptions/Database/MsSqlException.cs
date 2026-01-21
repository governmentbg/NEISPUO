namespace NeispuoExtension.Exceptions.Database
{
    using System;

    using Messages = ExceptionMessages.Database;

    public class MsSqlException : DatabaseException
    {
        public MsSqlException()
            : base(Messages.MsSq)
        {
        }

        public MsSqlException(string message)
            : base(message)
        {
        }

        public MsSqlException(Exception innerException)
            : base(Messages.MsSq, innerException)
        {
        }

        public MsSqlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
