namespace SB.Domain;

using System;

public partial interface IOffDaysQueryRepository
{
    public record GetAllVO(
        int OffDayId,
        DateTime From,
        DateTime To,
        string Description,
        bool IsForAllClasses,
        string[] BasicClassNames,
        string[] ClassBookNames);
}
