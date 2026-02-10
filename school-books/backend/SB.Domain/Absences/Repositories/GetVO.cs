namespace SB.Domain;

using System;

public partial interface IAbsencesQueryRepository
{
    public record GetVO(
        int AbsenceId,
        DateTime CreateDate,
        int CreatedBySysUserId);
}
