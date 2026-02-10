namespace SB.Domain;

using System;

public partial interface IBookVerificationQueryRepository
{
    public record GetOffDayVO(
        int ClassBookId,
        DateTime From,
        DateTime To,
        string Description);
}
