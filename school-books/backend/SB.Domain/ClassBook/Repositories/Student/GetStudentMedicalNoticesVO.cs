namespace SB.Domain;

using System;

public partial interface IStudentMedicalNoticesQueryRepository
{
    public record GetStudentMedicalNoticesVO(
        int HisMedicalNoticeId,
        string NrnMedicalNotice,
        string Pmi,
        DateTime AuthoredOn,
        DateTime FromDate,
        DateTime ToDate);
}
