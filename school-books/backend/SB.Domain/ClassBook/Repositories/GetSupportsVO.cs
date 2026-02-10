namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetSupportsVO(
        string[] Students,
        string Difficulties,
        string? Description,
        string? ExpectedResult,
        DateTime EndDate,
        string[] Teachers,
        GetSupportsVOActivity[] Activities);

    public record GetSupportsVOActivity(
        string ActivityType,
        DateTime? Date,
        string? Target,
        string? Result);
}
