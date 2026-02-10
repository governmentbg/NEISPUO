namespace SB.Domain;

using System;

public partial interface ISpbsBookRecordsQueryRepository
{
    public record GetAbsenceVO(
        int OrderNum,
        DateTime AbsenceDate,
        string AbsenceReason);
}
