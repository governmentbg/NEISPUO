namespace SB.Domain;

public class NvoExamDutyProtocolSupervisor
{
    // EF constructor
    private NvoExamDutyProtocolSupervisor()
    {
        this.NvoExamDutyProtocol = null!;
    }

    public NvoExamDutyProtocolSupervisor(
        NvoExamDutyProtocol nvoExamDutyProtocol,
        int personId)
    {
        this.NvoExamDutyProtocol = nvoExamDutyProtocol;
        this.PersonId = personId;
    }

    public int NvoExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public NvoExamDutyProtocol NvoExamDutyProtocol { get; private set; }
}
