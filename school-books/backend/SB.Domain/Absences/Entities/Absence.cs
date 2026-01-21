namespace SB.Domain;

using System;

public class Absence : IAggregateRoot
{
    private const int SchoolBooksSysUserId = 3;

    // EF constructor
    private Absence()
    {
    }

    public Absence(
        int schoolYear,
        int classBookId,
        int personId,
        AbsenceType type,
        SchoolTerm term,
        DateTime date,
        int scheduleLessonId,
        int? teacherAbsenceId,
        int? excusedReasonId,
        string? excusedReasonComment,
        int createdBySysUserId)
    {
        if (type != AbsenceType.Excused && (excusedReasonId != null || !string.IsNullOrEmpty(excusedReasonComment)))
        {
            throw new DomainValidationException("ExcusedReason and ExcusedReasonComment are allowed only for excused absences");
        }

        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.Date = date;
        this.Type = type;
        this.Term = term;
        this.ScheduleLessonId = scheduleLessonId;
        this.TeacherAbsenceId = teacherAbsenceId;
        this.ExcusedReasonId = excusedReasonId;
        this.ExcusedReasonComment = excusedReasonComment;
        this.IsReadFromParent = false;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int AbsenceId { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public DateTime Date { get; private set; }

    public AbsenceType Type { get; private set; }

    public SchoolTerm Term { get; private set; }

    public int ScheduleLessonId { get; private set; }

    public int? TeacherAbsenceId { get; private set; }

    public int? HisMedicalNoticeId { get; private set; }

    public int? ExcusedReasonId { get; private set; }

    public string? ExcusedReasonComment { get; private set; }

    public bool IsReadFromParent { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public void Excuse(int? excusedReasonId, string? excusedReasonComment, int modifiedBySysUserId)
    {
        if (this.Type != AbsenceType.Unexcused)
        {
            throw new DomainValidationException("Only unexcused absences can be excused.");
        }

        this.Type = AbsenceType.Excused;
        this.ExcusedReasonId = excusedReasonId;
        this.ExcusedReasonComment = excusedReasonComment;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void ConvertToLate(int modifiedBySysUserId)
    {
        if (this.Type != AbsenceType.Unexcused)
        {
            throw new DomainValidationException("Only unexcused absences can be converted to late.");
        }

        this.Type = AbsenceType.Late;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void ExcuseWithHisMedicalNotice(
        int hisMedicalNoticeId,
        string nrnMedicalNotice,
        string pmi,
        DateTime authoredOn,
        DateTime fromDate,
        DateTime toDate)
    {
        this.HisMedicalNoticeId = hisMedicalNoticeId;
        this.Type = AbsenceType.Excused;
        this.ExcusedReasonId = AbsenceReason.MedicalAbsenceReasonId;
        this.ExcusedReasonComment =
            fromDate.Date == toDate.Date
                ? $"Автоматично уважено с електронна медицинска бележка {nrnMedicalNotice}/{authoredOn:dd.MM.yyyy} за дата {fromDate:dd.MM.yyyy}. УИН на лекаря - {pmi}."
                : $"Автоматично уважено с електронна медицинска бележка {nrnMedicalNotice}/{authoredOn:dd.MM.yyyy} за периода {fromDate:dd.MM.yyyy} - {toDate:dd.MM.yyyy}. УИН на лекаря - {pmi}.";
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = SchoolBooksSysUserId;
    }

    public string EmailTag
    {
        get
        {
            return $"absence:{this.SchoolYear}:{this.AbsenceId}";
        }
    }

    public string PushNotificationTag
    {
        get
        {
            return $"absence-push:{this.SchoolYear}:{this.AbsenceId}";
        }
    }

    public void SetAsReadFromParent()
    {
        this.IsReadFromParent = true;
    }
}
