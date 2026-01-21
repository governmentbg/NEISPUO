namespace NeispuoExtension.Exceptions.Http
{
    using System;

    using Messages = ExceptionMessages.Http;

    public class ForbiddenException : HttpException
    {
        public ForbiddenException()
            : base(Messages.Forbidden)
        {
        }

        public ForbiddenException(string message)
            : base(message)
        {
        }

        public ForbiddenException(Exception innerException)
            : base(Messages.Forbidden, innerException)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
