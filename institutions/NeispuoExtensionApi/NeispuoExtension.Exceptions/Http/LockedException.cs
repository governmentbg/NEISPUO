namespace NeispuoExtension.Exceptions.Http
{
    using System;

    using Messages = ExceptionMessages.Http;

    public class LockedException : HttpException
    {
        public LockedException()
            : base(Messages.Locked)
        {
        }

        public LockedException(string message)
            : base(message)
        {
        }

        public LockedException(Exception innerException)
            : base(Messages.Locked, innerException)
        {
        }

        public LockedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
