namespace SB.Domain;

using System;

public partial interface IStateExamsAdmProtocolQueryRepository
{
    public record GetVO(
        int StateExamsAdmProtocolId,
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
