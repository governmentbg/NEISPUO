namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetSupportsVO(
        int SupportId,
        string Difficulties,
        string Activities,
        DateTime EndDate);
}
