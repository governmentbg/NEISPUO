namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class GradeChangeExamsAdmProtocolStudent
{
    // EF constructor
    private GradeChangeExamsAdmProtocolStudent()
    {
        this.GradeChangeExamsAdmProtocol = null!;
    }

    public GradeChangeExamsAdmProtocolStudent(
        GradeChangeExamsAdmProtocol gradeChangeExamsAdmProtocol,
        int classId,
        int personId,
        (int subjectId, int subjectTypeId)[] subjects)
    {
        this.GradeChangeExamsAdmProtocol = gradeChangeExamsAdmProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
        this.SetSubjects(subjects);
    }

    public int SchoolYear { get; private set; }
    public int GradeChangeExamsAdmProtocolId { get; private set; }
    public int ClassId { get; private set; }
    public int PersonId { get; private set; }

    // relations
    public GradeChangeExamsAdmProtocol GradeChangeExamsAdmProtocol { get; private set; }

    private readonly List<GradeChangeExamsAdmProtocolStudentSubject> subjects = new();
    public IReadOnlyCollection<GradeChangeExamsAdmProtocolStudentSubject> Subjects => this.subjects.AsReadOnly();

    public void UpdateData((int subjectId, int subjectTypeId)[] subjects)
    {
        this.SetSubjects(subjects);
    }

    private void SetSubjects((int subjectId, int subjectTypeId)[] subjects)
    {
        if (subjects == null)
        {
            throw new ArgumentNullException(nameof(subjects));
        }

        this.subjects.Clear();
        this.subjects.AddRange(subjects
            .Select(id => new GradeChangeExamsAdmProtocolStudentSubject(this, id.subjectId, id.subjectTypeId)));
    }
}
