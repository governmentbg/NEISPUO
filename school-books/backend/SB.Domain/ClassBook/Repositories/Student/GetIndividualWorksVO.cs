namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetIndividualWorksVO(
        DateTime Date,
        string IndividualWorkActivity,
        string CreatedBySysUserFullName);
}
