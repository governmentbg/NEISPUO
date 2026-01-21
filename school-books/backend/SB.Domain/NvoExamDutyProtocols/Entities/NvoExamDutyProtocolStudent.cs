namespace SB.Domain;

public class NvoExamDutyProtocolStudent
{
    // EF constructor
    private NvoExamDutyProtocolStudent()
    {
        this.NvoExamDutyProtocol = null!;
    }

    public NvoExamDutyProtocolStudent(
        NvoExamDutyProtocol nvoExamDutyProtocol,
        int classId,
        int personId)
    {
        this.NvoExamDutyProtocol = nvoExamDutyProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
    }

    public int NvoExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }
    

    // relations
    public NvoExamDutyProtocol NvoExamDutyProtocol { get; private set; }
}
