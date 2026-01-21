namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetSupportsVO(
        int SupportId,
        string Difficulties,
        string Activities,
        DateTime EndDate);
}
