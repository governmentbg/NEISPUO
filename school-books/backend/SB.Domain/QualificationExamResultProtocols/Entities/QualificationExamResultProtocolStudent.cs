namespace SB.Domain;

public class QualificationExamResultProtocolStudent
{
    // EF constructor
    private QualificationExamResultProtocolStudent()
    {
        this.QualificationExamResultProtocol = null!;
    }

    public QualificationExamResultProtocolStudent(
        QualificationExamResultProtocol qualificationExamResultProtocol,
        int classId,
        int personId)
    {
        this.QualificationExamResultProtocol = qualificationExamResultProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
    }

    public int QualificationExamResultProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }
    

    // relations
    public QualificationExamResultProtocol QualificationExamResultProtocol { get; private set; }
}
