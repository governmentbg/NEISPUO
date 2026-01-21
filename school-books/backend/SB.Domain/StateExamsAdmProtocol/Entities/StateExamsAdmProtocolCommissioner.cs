namespace SB.Domain;

public class StateExamsAdmProtocolCommissioner
{
    // EF constructor
    private StateExamsAdmProtocolCommissioner()
    {
        this.StateExamsAdmProtocol = null!;
    }

    public StateExamsAdmProtocolCommissioner(
        StateExamsAdmProtocol stateExamsAdmProtocol,
        int personId,
        bool isChairman,
        int orderNum)
    {
        this.StateExamsAdmProtocol = stateExamsAdmProtocol;
        this.PersonId = personId;
        this.IsChairman = isChairman;
        this.OrderNum = orderNum;
    }

    public int SchoolYear { get; private set; }
    public int StateExamsAdmProtocolId { get; private set; }
    public int PersonId { get; private set; }
    public bool IsChairman { get; private set; }
    public int OrderNum { get; private set; }

    // relations
    public StateExamsAdmProtocol StateExamsAdmProtocol { get; private set; }
}
