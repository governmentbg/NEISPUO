namespace NeispuoExtension.Exceptions.Database
{
    using System;

    using Messages = ExceptionMessages.Database;

    public class DatabaseException : NeispuoExtensionException
    {
        public DatabaseException()
            : base(Messages.BaseDatabase)
        {
        }

        public DatabaseException(string message)
            : base(message)
        {
        }

        public DatabaseException(Exception innerException)
            : base(Messages.BaseDatabase, innerException)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
