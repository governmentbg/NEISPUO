namespace SB.Domain;

using System;

public partial interface IOffDaysQueryRepository
{
    public record GetAllForRebuildVO(
        int OffDayId,
        string Description,
        DateTime From,
        DateTime To,
        bool IsForAllClasses,
        bool IsPgOffProgramDay,
        int[] BasicClassIds,
        int[] ClassBookIds);
}
