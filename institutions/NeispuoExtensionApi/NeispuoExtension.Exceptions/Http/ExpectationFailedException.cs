namespace NeispuoExtension.Exceptions.Http
{
    using System;

    using Messages = ExceptionMessages.Http;

    public class ExpectationFailedException : HttpException
    {
        public ExpectationFailedException()
            : base(Messages.ExpectationFailed)
        {
        }

        public ExpectationFailedException(string message)
            : base(message)
        {
        }

        public ExpectationFailedException(Exception innerException)
            : base(Messages.ExpectationFailed, innerException)
        {
        }

        public ExpectationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
