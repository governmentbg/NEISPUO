namespace SB.Domain;
public record CreateHighSchoolCertificateProtocolStudentCommandStudent
{
    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
}
