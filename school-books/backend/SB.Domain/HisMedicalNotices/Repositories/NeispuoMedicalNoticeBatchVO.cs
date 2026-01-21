namespace SB.Domain;

using System;

public partial interface IHisMedicalNoticesQueryRepository
{
    public record NeispuoMedicalNoticeBatchVO(
        NeispuoMedicalNoticeVO[] MedicalNotices,
        bool HasMore);

    public record NeispuoMedicalNoticeVO(
        int HisMedicalNoticeId,
        string NrnMedicalNotice,
        string Pmi,
        DateTime AuthoredOn,
        DateTime FromDate,
        DateTime ToDate,
        int PersonId,
        int[] SchoolYears);
}
