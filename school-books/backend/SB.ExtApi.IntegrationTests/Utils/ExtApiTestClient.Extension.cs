namespace SB.ExtApi.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

public enum TestExtSystem
{
    SchoolBooksProvider,
    ScheduleProvider,
    HIS,
}

public partial class ExtApiTestClient
{
    private TestExtSystem testExtSystem;
    public ExtApiTestClient(HttpClient httpClient, TestExtSystem testExtSystem)
        : this(httpClient)
    {
        this.testExtSystem = testExtSystem;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
    {
        request.Headers.TryAddWithoutValidation(
            "X-Client-Cert",
            this.testExtSystem switch
            {
                TestExtSystem.SchoolBooksProvider => RequestCertificates.SchoolBooksProviderCertificateString,
                TestExtSystem.ScheduleProvider => RequestCertificates.ScheduleProviderCertificateString,
                TestExtSystem.HIS => RequestCertificates.HISCertificateString,
                _ => throw new Exception($"Unknown {nameof(TestExtSystem)}")
            });
        request.Headers.TryAddWithoutValidation("X-Forwarded-Proto", "HTTPS");
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", "127.0.0.1");
    }
}

public partial class ApiException<TResult> : ApiException
{
    public ApiException(
        string message,
        int statusCode,
        string response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        ValidationErrorResponse result,
        Exception innerException)
        : base(message + "\n" + FormatError(result), statusCode, response, headers, innerException)
    {
    }

    private static string FormatError(ValidationErrorResponse error)
    {
        return
$@"
Errors: {string.Join("\n", error.Errors)}
ErrorMessages: {string.Join("\n", error.ErrorMessages)}
SysErrorMessage: {error.SysErrorMessage}";
    }
}
