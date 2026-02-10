namespace MonProjects.Exceptions
{
    using Exceptions.Contracts;
    using Exceptions.Models;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    public class DatabaseException : Exception, ICustomExceptionCreator
    {
        public DatabaseException()
        {
        }

        public DatabaseException(string message)
            : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public async Task CreateErrorMessage(string message, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = StatusCodes.Status409Conflict;
            await httpResponse.WriteAsync(new ErrorMessageDetailsModel
            {
                StatusCode = httpResponse.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
