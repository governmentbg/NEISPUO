namespace SB.Domain;

public class GradeChangeExamsAdmProtocolStudentSubject
{
    // EF constructor
    private GradeChangeExamsAdmProtocolStudentSubject()
    {
        this.GradeChangeExamsAdmProtocolStudent = null!;
    }

    public GradeChangeExamsAdmProtocolStudentSubject(
        GradeChangeExamsAdmProtocolStudent gradeChangeExamsAdmProtocolStudent,
        int subjectId,
        int subjectTypeId)
    {
        this.GradeChangeExamsAdmProtocolStudent = gradeChangeExamsAdmProtocolStudent;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
    }

    public int SchoolYear { get; private set; }
    public int GradeChangeExamsAdmProtocolId { get; private set; }
    public int ClassId { get; private set; }
    public int PersonId { get; private set; }
    public int SubjectId { get; private set; }
    public int SubjectTypeId { get; private set; }

    // relations
    public GradeChangeExamsAdmProtocolStudent GradeChangeExamsAdmProtocolStudent { get; private set; }
}
