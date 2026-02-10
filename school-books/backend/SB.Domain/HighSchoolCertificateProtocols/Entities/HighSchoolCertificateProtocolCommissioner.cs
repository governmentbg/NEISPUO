namespace SB.Domain;

public class HighSchoolCertificateProtocolCommissioner
{
    // EF constructor
    private HighSchoolCertificateProtocolCommissioner()
    {
        this.HighSchoolCertificateProtocol = null!;
    }

    public HighSchoolCertificateProtocolCommissioner(
        HighSchoolCertificateProtocol highSchoolCertificateProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.HighSchoolCertificateProtocol = highSchoolCertificateProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int HighSchoolCertificateProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public HighSchoolCertificateProtocol HighSchoolCertificateProtocol { get; private set; }
}
