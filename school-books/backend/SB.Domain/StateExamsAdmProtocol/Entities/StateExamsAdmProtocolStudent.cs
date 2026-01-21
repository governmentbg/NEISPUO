namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class StateExamsAdmProtocolStudent
{
    // EF constructor
    private StateExamsAdmProtocolStudent()
    {
        this.StateExamsAdmProtocol = null!;
    }

    public StateExamsAdmProtocolStudent(
        StateExamsAdmProtocol stateExamsAdmProtocol,
        int classId,
        int personId,
        bool hasFirstMandatorySubject,
        int? secondMandatorySubjectId,
        int? secondMandatorySubjectTypeId,
        (int subjectId, int subjectTypeId)[] additionalSubjects,
        (int subjectId, int subjectTypeId)[] qualificationSubjects)
    {
        this.StateExamsAdmProtocol = stateExamsAdmProtocol;
        this.ClassId = classId;
        this.PersonId = personId;
        this.HasFirstMandatorySubject = hasFirstMandatorySubject;
        this.SecondMandatorySubjectId = secondMandatorySubjectId;
        this.SecondMandatorySubjectTypeId = secondMandatorySubjectTypeId;
        this.SetSubjects(additionalSubjects, qualificationSubjects);
    }

    public int SchoolYear { get; private set; }
    public int StateExamsAdmProtocolId { get; private set; }
    public int ClassId { get; private set; }
    public int PersonId { get; private set; }
    public bool HasFirstMandatorySubject { get; private set; }
    public int? SecondMandatorySubjectId { get; private set; }
    public int? SecondMandatorySubjectTypeId { get; private set; }

    // relations
    public StateExamsAdmProtocol StateExamsAdmProtocol { get; private set; }

    private readonly List<StateExamsAdmProtocolStudentSubject> subjects = new();
    public IReadOnlyCollection<StateExamsAdmProtocolStudentSubject> Subjects => this.subjects.AsReadOnly();

    public void UpdateData(
        bool hasFirstMandatorySubject,
        int? secondMandatorySubjectId,
        int? secondMandatorySubjectTypeId,
        (int subjectId, int subjectTypeId)[] additionalSubjectIds,
        (int subjectId, int subjectTypeId)[] qualificationSubjectIds)
    {
        this.HasFirstMandatorySubject = hasFirstMandatorySubject;
        this.SecondMandatorySubjectId = secondMandatorySubjectId;
        this.SecondMandatorySubjectTypeId = secondMandatorySubjectTypeId;
        this.SetSubjects(additionalSubjectIds, qualificationSubjectIds);
    }

    private void SetSubjects((int subjectId, int subjectTypeId)[] additionalSubjectIds, (int subjectId, int subjectTypeId)[] qualificationSubjectIds)
    {
        if (additionalSubjectIds == null)
        {
            throw new ArgumentNullException(nameof(additionalSubjectIds));
        }

        if (qualificationSubjectIds == null)
        {
            throw new ArgumentNullException(nameof(qualificationSubjectIds));
        }

        this.subjects.Clear();
        this.subjects.AddRange(additionalSubjectIds
            .Select(id => new StateExamsAdmProtocolStudentSubject(this, id.subjectId, id.subjectTypeId, true))
            .Concat(qualificationSubjectIds!
                .Select(id => new StateExamsAdmProtocolStudentSubject(this, id.subjectId, id.subjectTypeId, false))));
    }
}
