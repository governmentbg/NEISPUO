namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentSupportsVO(
        string Difficulties,
        string? Description,
        string? ExpectedResult,
        DateTime EndDate,
        string[] Teachers,
        GetStudentSupportsVOActivity[] Activities);

    public record GetStudentSupportsVOActivity(
        string ActivityType,
        DateTime? Date,
        string? Target,
        string? Result);
}
