namespace SB.Domain;
public partial interface IHighSchoolCertificateProtocolQueryRepository
{
    public record GetStudentAllVO(
        string ClassName,
        int ClassId,
        int PersonId,
        string StudentName);
}
