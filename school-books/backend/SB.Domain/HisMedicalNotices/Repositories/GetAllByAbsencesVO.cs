namespace SB.Domain;

using System;

public partial interface IPersonMedicalNoticeQueryRepository
{
    public record GetAllByAbsencesVO(
        int PersonId,
        int HisMedicalNoticeId,
        string NrnMedicalNotice,
        string Pmi,
        DateTime AuthoredOn,
        DateTime FromDate,
        DateTime ToDate);
}
