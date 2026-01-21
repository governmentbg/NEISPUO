namespace SB.Domain;

public class GradeChangeExamsAdmProtocolCommissioner
{
    // EF constructor
    private GradeChangeExamsAdmProtocolCommissioner()
    {
        this.GradeChangeExamsAdmProtocol = null!;
    }

    public GradeChangeExamsAdmProtocolCommissioner(
        GradeChangeExamsAdmProtocol gradeChangeExamsAdmProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.GradeChangeExamsAdmProtocol = gradeChangeExamsAdmProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int GradeChangeExamsAdmProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public GradeChangeExamsAdmProtocol GradeChangeExamsAdmProtocol { get; private set; }
}
