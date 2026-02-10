namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentParentMeetingsVO(
        DateTime Date,
        string Title,
        string? Description
    );
}
