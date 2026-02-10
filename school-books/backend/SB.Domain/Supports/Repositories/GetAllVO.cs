namespace SB.Domain;

using System;

public partial interface ISupportsQueryRepository
{
    public record GetAllVO(
        int SupportId,
        bool IsForAllStudents,
        string Difficulties,
        string Students,
        string Activities,
        DateTime CreateDate,
        DateTime EndDate);
}
