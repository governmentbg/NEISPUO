namespace SB.Domain;
public partial interface IHighSchoolCertificateProtocolQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId);
}
