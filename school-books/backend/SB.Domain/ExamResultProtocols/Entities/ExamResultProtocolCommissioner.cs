namespace SB.Domain;

public class ExamResultProtocolCommissioner
{
    // EF constructor
    private ExamResultProtocolCommissioner()
    {
        this.ExamResultProtocol = null!;
    }

    public ExamResultProtocolCommissioner(
        ExamResultProtocol examResultProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.ExamResultProtocol = examResultProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int ExamResultProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public ExamResultProtocol ExamResultProtocol { get; private set; }
}
