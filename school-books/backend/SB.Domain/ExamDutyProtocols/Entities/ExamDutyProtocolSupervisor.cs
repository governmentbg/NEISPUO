namespace SB.Domain;

public class ExamDutyProtocolSupervisor
{
    // EF constructor
    private ExamDutyProtocolSupervisor()
    {
        this.ExamDutyProtocol = null!;
    }

    public ExamDutyProtocolSupervisor(
        ExamDutyProtocol examDutyProtocol,
        int personId)
    {
        this.ExamDutyProtocol = examDutyProtocol;
        this.PersonId = personId;
    }

    public int ExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public ExamDutyProtocol ExamDutyProtocol { get; private set; }
}
