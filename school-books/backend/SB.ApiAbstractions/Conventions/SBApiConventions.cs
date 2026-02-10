namespace SB.ApiAbstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

#pragma warning disable
#nullable disable

public static class SBApiConventions
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status508LoopDetected)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    public static void Any(params object[] p)
    {
    }
}
