namespace SB.Domain;

using System;

public partial interface IGradeChangeExamsAdmProtocolQueryRepository
{
    public record GetVO(
        int GradeChangeExamsAdmProtocolId,
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
