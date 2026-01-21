namespace SB.Domain;

using System;

public class FirstGradeResult : IAggregateRoot
{
    // EF constructor
    protected FirstGradeResult()
    {
        this.Version = null!;
    }

    public FirstGradeResult(
        int schoolYear,
        int personId,
        int classBookId,
        QualitativeGrade? qualitativeGrade,
        SpecialNeedsGrade? specialGrade,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.PersonId = personId;
        this.ClassBookId = classBookId;

        if (!(qualitativeGrade != null && specialGrade == null) &&
            !(specialGrade != null && qualitativeGrade == null))
        {
            throw new DomainValidationException("Exactly one of qualitativeGrade and specialGrade must have a value");
        }

        this.QualitativeGrade = qualitativeGrade;
        this.SpecialGrade = specialGrade;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifiedBySysUserId = createdBySysUserId;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }

    public int FirstGradeResultId { get; private set; }

    public int PersonId { get; private set; }

    public int ClassBookId { get; private set; }

    public QualitativeGrade? QualitativeGrade { get; private set; }

    public SpecialNeedsGrade? SpecialGrade { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    public void UpdateData(
        QualitativeGrade? qualitativeGrade,
        SpecialNeedsGrade? specialGrade,
        int modifiedBySysUserId)
    {
        if (!(qualitativeGrade != null && specialGrade == null) &&
            !(specialGrade != null && qualitativeGrade == null))
        {
            throw new DomainValidationException("Exactly one of qualitativeGrade and specialGrade must have a value");
        }

        if ((qualitativeGrade != null && this.SpecialGrade != null) ||
            (specialGrade != null && this.QualitativeGrade != null))
        {
            throw new DomainValidationException("Cannot change the grade type of existing first grade result");
        }

        this.QualitativeGrade = qualitativeGrade;
        this.SpecialGrade = specialGrade;

        this.ModifiedBySysUserId = modifiedBySysUserId;
        this.ModifyDate = DateTime.Now;
    }
}
