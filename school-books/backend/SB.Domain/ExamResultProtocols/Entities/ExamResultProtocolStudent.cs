namespace SB.Domain;

public class ExamResultProtocolStudent
{
    // EF constructor
    private ExamResultProtocolStudent()
    {
        this.ExamResultProtocol = null!;
    }

    public ExamResultProtocolStudent(
        ExamResultProtocol examResultProtocol,
        int classId,
        int personId)
    {
        this.ExamResultProtocol = examResultProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
    }

    public int ExamResultProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }
    

    // relations
    public ExamResultProtocol ExamResultProtocol { get; private set; }
}
