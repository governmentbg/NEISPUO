namespace SB.Domain;

public class ExamResultProtocolClass
{
    // EF constructor
    private ExamResultProtocolClass()
    {
        this.ExamResultProtocol = null!;
    }

    public ExamResultProtocolClass(
        ExamResultProtocol examResultProtocol,
        int classId)
    {
        this.ExamResultProtocol = examResultProtocol;
        this.ClassId = classId;
    }

    public int ExamResultProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    // relations
    public ExamResultProtocol ExamResultProtocol { get; private set; }
}
