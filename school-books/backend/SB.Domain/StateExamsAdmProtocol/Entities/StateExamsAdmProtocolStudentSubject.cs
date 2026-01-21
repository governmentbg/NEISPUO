namespace SB.Domain;

public class StateExamsAdmProtocolStudentSubject
{
    // EF constructor
    private StateExamsAdmProtocolStudentSubject()
    {
        this.StateExamsAdmProtocolStudent = null!;
    }

    public StateExamsAdmProtocolStudentSubject(
        StateExamsAdmProtocolStudent stateExamsAdmProtocolStudent,
        int subjectId,
        int subjectTypeId,
        bool isAdditional)
    {
        this.StateExamsAdmProtocolStudent = stateExamsAdmProtocolStudent;
        this.SubjectId = subjectId;
        this.SubjectTypeId = subjectTypeId;
        this.IsAdditional = isAdditional;
    }

    public int SchoolYear { get; private set; }
    public int StateExamsAdmProtocolId { get; private set; }
    public int ClassId { get; private set; }
    public int PersonId { get; private set; }
    public int SubjectId { get; private set; }
    public int SubjectTypeId { get; private set; }
    public bool IsAdditional { get; private set; }

    // relations
    public StateExamsAdmProtocolStudent StateExamsAdmProtocolStudent { get; private set; }
}
