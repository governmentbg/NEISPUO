namespace MonProjects.Exceptions.Contracts
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface ICustomExceptionCreator
    {
        Task CreateErrorMessage(string message, HttpResponse httpResponse);
    }
}
