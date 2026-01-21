namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetOffDatesPgVO(
        DateTime Date,
        bool IsPgOffProgramDay);
}
