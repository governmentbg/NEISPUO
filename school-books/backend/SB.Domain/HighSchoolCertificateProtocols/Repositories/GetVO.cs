namespace SB.Domain;

using System;

public partial interface IHighSchoolCertificateProtocolQueryRepository
{
    public record GetVO(
        int HighSchoolCertificateProtocolId,
        HighSchoolCertificateProtocolStage Stage,
        string? ProtocolNum,
        DateTime? ProtocolDate,
        DateTime CommissionMeetingDate,
        string CommissionNominationOrderNumber,
        DateTime CommissionNominationOrderDate,
        string? ExamSession,
        int DirectorPersonId,
        int CommissionChairman,
        int[] CommissionMembers);
}
