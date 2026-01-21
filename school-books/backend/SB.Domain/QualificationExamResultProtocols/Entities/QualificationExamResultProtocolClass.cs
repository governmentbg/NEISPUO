namespace SB.Domain;

public class QualificationExamResultProtocolClass
{
    // EF constructor
    private QualificationExamResultProtocolClass()
    {
        this.QualificationExamResultProtocol = null!;
    }

    public QualificationExamResultProtocolClass(
        QualificationExamResultProtocol examResultProtocol,
        int classId)
    {
        this.QualificationExamResultProtocol = examResultProtocol;
        this.ClassId = classId;
    }

    public int QualificationExamResultProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    // relations
    public QualificationExamResultProtocol QualificationExamResultProtocol { get; private set; }
}
