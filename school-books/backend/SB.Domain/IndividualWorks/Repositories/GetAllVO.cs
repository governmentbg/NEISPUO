namespace SB.Domain;

using System;

public partial interface IIndividualWorksQueryRepository
{
    public record GetAllVO(
        int IndividualWorkId,
        int PersonId,
        string FullName,
        bool IsTransferred,
        DateTime Date,
        string IndividualWorkActivity,
        string CreatedBySysUserFullName);
}
