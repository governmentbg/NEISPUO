namespace SB.Domain;

public class StateExamDutyProtocolSupervisor
{
    // EF constructor
    private StateExamDutyProtocolSupervisor()
    {
        this.StateExamDutyProtocol = null!;
    }

    public StateExamDutyProtocolSupervisor(
        StateExamDutyProtocol stateExamDutyProtocol, int personId)
    {
        this.StateExamDutyProtocol = stateExamDutyProtocol;
        this.PersonId = personId;
    }

    public int StateExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public StateExamDutyProtocol StateExamDutyProtocol { get; private set; }
}
