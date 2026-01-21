namespace SB.Domain;

using System;

public partial interface IOffDaysQueryRepository
{
    public record GetVO(
        int OffDayId,
        DateTime From,
        DateTime To,
        string Description,
        bool IsForAllClasses,
        int[] BasicClassIds,
        int[] ClassBookIds,
        bool IsPgOffProgramDay);
}
