namespace SB.Domain;

public class ExamDutyProtocolClass
{
    // EF constructor
    private ExamDutyProtocolClass()
    {
        this.ExamDutyProtocol = null!;
    }

    public ExamDutyProtocolClass(
        ExamDutyProtocol examDutyProtocol,
        int classId)
    {
        this.ExamDutyProtocol = examDutyProtocol;
        this.ClassId = classId;
    }

    public int ExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    // relations
    public ExamDutyProtocol ExamDutyProtocol { get; private set; }
}
