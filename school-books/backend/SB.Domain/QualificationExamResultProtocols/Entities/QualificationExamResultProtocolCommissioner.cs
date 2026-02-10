namespace SB.Domain;

public class QualificationExamResultProtocolCommissioner
{
    // EF constructor
    private QualificationExamResultProtocolCommissioner()
    {
        this.QualificationExamResultProtocol = null!;
    }

    public QualificationExamResultProtocolCommissioner(
        QualificationExamResultProtocol examResultProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.QualificationExamResultProtocol = examResultProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int QualificationExamResultProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public QualificationExamResultProtocol QualificationExamResultProtocol { get; private set; }
}
