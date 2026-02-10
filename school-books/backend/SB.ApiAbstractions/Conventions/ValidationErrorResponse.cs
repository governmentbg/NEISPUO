namespace SB.ApiAbstractions;

using System;

public class ValidationErrorResponse
{
    public string[] Errors { get; init; } = Array.Empty<string>();
    public string[] ErrorMessages { get; init; } = Array.Empty<string>();
    public string SysErrorMessage { get; init; } = string.Empty;
}
