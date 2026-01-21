namespace SB.Domain;

public class GraduationThesisDefenseProtocolCommissioner
{
    // EF constructor
    private GraduationThesisDefenseProtocolCommissioner()
    {
        this.GraduationThesisDefenseProtocol = null!;
    }

    public GraduationThesisDefenseProtocolCommissioner(
        GraduationThesisDefenseProtocol gradeChangeExamsAdmProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.GraduationThesisDefenseProtocol = gradeChangeExamsAdmProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int GraduationThesisDefenseProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public GraduationThesisDefenseProtocol GraduationThesisDefenseProtocol { get; private set; }
}
