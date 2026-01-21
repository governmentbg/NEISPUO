namespace SB.ExtApi;

using System;
using Microsoft.AspNetCore.Authorization;

public record ExtSystemAccessRequirementContext(
    ExtApiOptions Options,
    int ExtSystemId,
    int[] ExtSystemTypes,
    int? SysUserId);

public class ExtSystemAccessRequirement : IAuthorizationRequirement
{
    public required Func<ExtSystemAccessRequirementContext, bool> Assertion { get; init; }
}
