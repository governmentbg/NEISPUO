namespace SB.Domain;

using System;

public partial interface IRemarksQueryRepository
{
    public record GetVO(
        int RemarkId,
        int CurriculumId,
        int PersonId,
        DateTime Date,
        string Description);
}
