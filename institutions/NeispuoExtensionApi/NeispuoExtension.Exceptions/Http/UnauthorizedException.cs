namespace NeispuoExtension.Exceptions.Http
{
    using System;

    using Messages = ExceptionMessages.Http;

    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException()
            : base(Messages.Unauthorized)
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException(Exception innerException)
            : base(Messages.Unauthorized, innerException)
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
