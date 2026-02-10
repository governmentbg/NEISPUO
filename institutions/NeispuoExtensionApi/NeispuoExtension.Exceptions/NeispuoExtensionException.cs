namespace NeispuoExtension.Exceptions
{
    using System;

    using Messages = ExceptionMessages;

    public class NeispuoExtensionException : Exception
    {
        public NeispuoExtensionException()
            : base(Messages.Base)
        {
        }

        public NeispuoExtensionException(string message)
            : base(message)
        {
        }

        public NeispuoExtensionException(Exception innerException)
            : base(Messages.Base, innerException)
        {
        }

        public NeispuoExtensionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
