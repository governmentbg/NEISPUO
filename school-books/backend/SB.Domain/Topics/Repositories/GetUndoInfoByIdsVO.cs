namespace SB.Domain;

using System;

public partial interface ITopicsQueryRepository
{
    public record GetUndoInfoByIdsVO(
        int TopicId,
        DateTime CreateDate,
        int CreatedBySysUserId);
}
