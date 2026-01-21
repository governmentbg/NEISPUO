namespace SB.Domain;
public class HighSchoolCertificateProtocolStudent
{
    // EF constructor
    private HighSchoolCertificateProtocolStudent()
    {
        this.HighSchoolCertificateProtocol = null!;
    }

    public HighSchoolCertificateProtocolStudent(
        HighSchoolCertificateProtocol highSchoolCertificateProtocol,
        int classId,
        int personId)
    {
        this.HighSchoolCertificateProtocol = highSchoolCertificateProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }
    public int HighSchoolCertificateProtocolId { get; private set; }
    public int ClassId { get; private set; }
    public int PersonId { get; private set; }

    // relations
    public HighSchoolCertificateProtocol HighSchoolCertificateProtocol { get; private set; }
}
