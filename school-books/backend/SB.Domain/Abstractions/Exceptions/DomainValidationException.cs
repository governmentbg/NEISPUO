namespace SB.Domain;

using System;
using System.Linq;

public class DomainValidationException : DomainException
{
    public DomainValidationException(string[] errors, string[] errorMessages)
        : base($"Domain validation failed. Errors = {ArrayToString(errors)}, ErrorMessages = {ArrayToString(errorMessages)}.")
    {
        this.Errors = errors;
        this.ErrorMessages = errorMessages;
    }

    public DomainValidationException(string message)
        : this(new[] { message }, Array.Empty<string>())
    {
    }

    public DomainValidationException(string[] errors, string[] errorMessages, string sysErrorMessage)
        : this(errors, errorMessages)
    {
        this.SysErrorMessage = sysErrorMessage;
    }

    public string[] Errors { get; init; }
    public string[] ErrorMessages { get; init; }
    public string SysErrorMessage { get; init; } = string.Empty;

    static string ArrayToString(string[]? arr)
            => $"[{string.Join(", ", (arr ?? Array.Empty<string>()).Select(e => $"\"{e}\""))}]";
}
