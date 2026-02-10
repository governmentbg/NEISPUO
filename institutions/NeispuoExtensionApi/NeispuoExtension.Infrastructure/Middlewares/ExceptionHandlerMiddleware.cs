namespace NeispuoExtension.Infrastructure.Middlewares
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    using Settings;
    using Exceptions.Http;

    internal class ExceptionHandlerMiddleware
    {
        private const string CONTENT_TYPE_KEY = "Content-Type";
        private const string CONTENT_TYPE_VALUE = "text/plain; charset=utf-8";

        private readonly RequestDelegate next;
        private readonly string baseErrorLogDirectory;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            IOptions<ApplicationSettings> options)
        {
            this.next = next;
            this.baseErrorLogDirectory = options.Value.BaseErrorLogDirectory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await this.next(context);
            }
            catch (UnauthorizedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Headers.Add(CONTENT_TYPE_KEY, CONTENT_TYPE_VALUE);
                await context.Response.WriteAsync("unauthorized");
            }
            catch (ForbiddenException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Headers.Add(CONTENT_TYPE_KEY, CONTENT_TYPE_VALUE);
                await context.Response.WriteAsync("forbidden");
            }
            catch (ExpectationFailedException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                context.Response.Headers.Add(CONTENT_TYPE_KEY, CONTENT_TYPE_VALUE);
                await context.Response.WriteAsync(ex.Message);
            }
            catch (LockedException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Locked;
                context.Response.Headers.Add(CONTENT_TYPE_KEY, CONTENT_TYPE_VALUE);
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await this.LogErrorAsync(ex, context);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Headers.Add(CONTENT_TYPE_KEY, CONTENT_TYPE_VALUE);
                await context.Response.WriteAsync(String.Empty);
            }
        }

        private async Task LogErrorAsync(Exception exception, HttpContext context)
        {
            DateTime dateTime = DateTime.Now;

            string dateTimeRow = String.Concat(" - DateTime: ", dateTime);
            string ipAddressRow = String.Concat(" - IpAddress: ", context.Connection.RemoteIpAddress.ToString());
            string methodRow = String.Concat(" - Method: ", context.Request.Method);
            string pathRow = String.Concat(" - Path: ", context.Request.Path.Value);
            string exceptionsRow = String.Concat(" - Exceptions: ", AggregateExceptionMessages(exception));
            string stackTraceRow = String.Concat(" - StackTrace: ", exception.StackTrace.Trim());
            string requestHeadersRow = String.Concat(" - RequestHeaders: ", AggregateRequestHeaders(context.Request.Headers));
            string requestBodyRow = String.Concat(" - RequestBody: ", await AggregateRequestBodyAsync(context.Request.Body));

            StringBuilder exceptionBuilder = new StringBuilder();
            exceptionBuilder.AppendLine(dateTimeRow);
            exceptionBuilder.AppendLine(ipAddressRow);
            exceptionBuilder.AppendLine(methodRow);
            exceptionBuilder.AppendLine(pathRow);
            exceptionBuilder.AppendLine(exceptionsRow);
            exceptionBuilder.AppendLine(stackTraceRow);
            exceptionBuilder.AppendLine(requestHeadersRow);
            exceptionBuilder.AppendLine(requestBodyRow);

            string currentDirectory = Directory.GetCurrentDirectory();
            string currentYear = dateTime.Year.ToString();
            string currentMonth = dateTime.Month.ToString("D2");
            string currentDay = dateTime.ToString("yyyy-MM-dd");

            string directory = Path.Combine(currentDirectory, this.baseErrorLogDirectory, currentYear, currentMonth, currentDay);
            string fileName = String.Concat(dateTime.ToString("HH-mm-ss"), "-", Guid.NewGuid().ToString()[..8], ".txt");
            string fileDirectory = Path.Combine(directory, fileName);

            Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(fileDirectory, exceptionBuilder.ToString());
        }

        private static StringBuilder AggregateExceptionMessages(Exception exception, int index = 1)
        {
            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine();
            messageBuilder.Append("   - ");
            messageBuilder.Append(index);
            messageBuilder.Append(". ");
            messageBuilder.Append(exception.GetType());
            messageBuilder.Append(" - ");
            messageBuilder.Append(exception.Message);

            if (exception.InnerException == null)
            {
                return messageBuilder;
            }

            StringBuilder innerExceptionMessage = AggregateExceptionMessages(exception.InnerException, ++index);

            return messageBuilder.Append(innerExceptionMessage);
        }

        private static StringBuilder AggregateRequestHeaders(IHeaderDictionary headers, int index = 1)
        {
            StringBuilder headerBuilder = new StringBuilder();

            foreach (var key in headers.Keys)
            {
                headerBuilder.AppendLine();
                headerBuilder.Append("   - ");
                headerBuilder.Append(index++);
                headerBuilder.Append(": ");
                headerBuilder.Append(key);
                headerBuilder.Append(": ");
                headerBuilder.Append(headers[key]);
            }

            return headerBuilder;
        }

        private async static Task<StringBuilder> AggregateRequestBodyAsync(Stream body)
        {
            body.Seek(0, SeekOrigin.Begin);

            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine();

            using (StreamReader bodyReader = new StreamReader(body))
            {
                string content = await bodyReader.ReadToEndAsync();
                bodyBuilder.Append(content);
            }

            return bodyBuilder;
        }
    }
}
