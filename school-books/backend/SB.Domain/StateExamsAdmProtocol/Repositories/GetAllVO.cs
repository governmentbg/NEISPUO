namespace SB.Domain;

using System;

public partial interface IStateExamsAdmProtocolQueryRepository
{
    public record GetAllVO(
        int AdmProtocolId,
        AdmProtocolType? ProtocolType,
        string? ProtocolNum,
        DateTime? ProtocolDate,
        string? ExamSession);
}
