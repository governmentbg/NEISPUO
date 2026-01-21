namespace SB.Domain;

using System;

public partial interface IPersonMedicalNoticeQueryRepository
{
    public record GetAllByClassBookVO(
        string StudentFullName,
        string? Badge,
        string NrnMedicalNotice,
        string Pmi,
        DateTime AuthoredOn,
        DateTime FromDate,
        DateTime ToDate);
}
