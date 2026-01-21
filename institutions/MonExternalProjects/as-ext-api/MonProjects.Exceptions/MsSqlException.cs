using System;

namespace MonProjects.Exceptions
{
    public class MsSqlException : DatabaseException
    {
        public MsSqlException()
        {
        }

        public MsSqlException(string message) 
            : base(message)
        {
        }

        public MsSqlException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
