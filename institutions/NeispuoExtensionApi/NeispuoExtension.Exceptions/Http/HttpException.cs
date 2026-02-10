namespace NeispuoExtension.Exceptions.Http
{
    using System;

    using Messages = ExceptionMessages.Http;

    public class HttpException : NeispuoExtensionException
    {
        public HttpException()
            : base(Messages.BaseHttp)
        {
        }

        public HttpException(string message)
            : base(message)
        {
        }

        public HttpException(Exception innerException)
            : base(Messages.BaseHttp, innerException)
        {
        }

        public HttpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
