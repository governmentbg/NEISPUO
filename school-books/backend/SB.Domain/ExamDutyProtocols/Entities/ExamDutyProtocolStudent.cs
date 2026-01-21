namespace SB.Domain;

public class ExamDutyProtocolStudent
{
    // EF constructor
    private ExamDutyProtocolStudent()
    {
        this.ExamDutyProtocol = null!;
    }

    public ExamDutyProtocolStudent(
        ExamDutyProtocol examDutyProtocol,
        int classId,
        int personId)
    {
        this.ExamDutyProtocol = examDutyProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
    }

    public int ExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }
    

    // relations
    public ExamDutyProtocol ExamDutyProtocol { get; private set; }
}
