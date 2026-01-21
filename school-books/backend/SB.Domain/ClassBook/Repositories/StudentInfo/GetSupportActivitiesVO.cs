namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetSupportActivitiesVO(
        string ActivityTypeDesc,
        DateTime? Date,
        string? Target,
        string? Result);
}
