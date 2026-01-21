namespace SB.Domain;

using System;

public partial interface ITopicsDplrQueryRepository
{
    public record GetUndoInfoByIdsVO(
        int TopicDplrId,
        DateTime CreateDate,
        int CreatedBySysUserId);
}
