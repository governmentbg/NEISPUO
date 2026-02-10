namespace SB.Domain;

using System;

public class PgResult : IAggregateRoot
{
    // EF constructor
    private PgResult()
    {
        this.Version = null!;
    }

    public PgResult(
        int schoolYear,
        int classBookId,
        int personId,
        int? subjectId,
        int? curriculumId,
        string? startSchoolYearResult,
        string? endSchoolYearResult,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.SubjectId = subjectId;
        this.CurriculumId = curriculumId;
        this.StartSchoolYearResult = startSchoolYearResult;
        this.EndSchoolYearResult = endSchoolYearResult;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int PgResultId { get; private set; }
    public int ClassBookId { get; private set; }
    public int PersonId { get; private set; }
    public int? SubjectId { get; private set; }
    public int? CurriculumId { get; private set; }
    public string? StartSchoolYearResult { get; private set; }
    public string? EndSchoolYearResult { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        string? startSchoolYearResult,
        string? endSchoolYearResult,
        int modifiedBySysUserId)
    {
        this.StartSchoolYearResult = startSchoolYearResult;
        this.EndSchoolYearResult = endSchoolYearResult;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

}
