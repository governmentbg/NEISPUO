namespace SB.Domain;

public class QualificationAcquisitionProtocolCommissioner
{
    // EF constructor
    private QualificationAcquisitionProtocolCommissioner()
    {
        this.QualificationAcquisitionProtocol = null!;
    }

    public QualificationAcquisitionProtocolCommissioner(
        QualificationAcquisitionProtocol AcquisitionProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.QualificationAcquisitionProtocol = AcquisitionProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int QualificationAcquisitionProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public QualificationAcquisitionProtocol QualificationAcquisitionProtocol { get; private set; }
}
