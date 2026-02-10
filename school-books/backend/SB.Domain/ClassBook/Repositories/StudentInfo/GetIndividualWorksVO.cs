namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetIndividualWorksVO(
        DateTime Date,
        string IndividualWorkActivity,
        string CreatedBySysUserFullName);
}
