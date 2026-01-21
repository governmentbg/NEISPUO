namespace SB.Domain;

using System;

public partial interface IHighSchoolCertificateProtocolQueryRepository
{
    public record GetAllVO(
        int HighSchoolCertificateProtocolId,
        string ProtocolNum,
        DateTime ProtocolDate,
        string ExamSession);
}
