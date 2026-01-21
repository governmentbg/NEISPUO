namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetParentMeetingsVO(
        DateTime Date,
        string Title,
        string? Description
    );
}
