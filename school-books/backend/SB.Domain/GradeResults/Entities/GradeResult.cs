namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class GradeResult : IAggregateRoot
{
    // EF constructor
    private GradeResult()
    {
        this.Version = null!;
    }

    public GradeResult(
        int schoolYear,
        int classBookId,
        int personId,
        GradeResultType initialResultType,
        int[] retakeExamCurriculumIds,
        GradeResultType? finalResultType,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.InitialResultType = initialResultType;
        this.FinalResultType = finalResultType;
        this.gradeResultSubjects.AddRange(
            retakeExamCurriculumIds.Select(
                curriculumId => new GradeResultSubject(this, curriculumId)
            )
        );

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;

        this.AssertCorrectData();
    }

    public int SchoolYear { get; private set; }

    public int GradeResultId { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public GradeResultType InitialResultType { get; private set; }

    public GradeResultType? FinalResultType { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<GradeResultSubject> gradeResultSubjects = new();
    public IReadOnlyCollection<GradeResultSubject> GradeResultSubjects => this.gradeResultSubjects.AsReadOnly();

    public void UpdateData(
        GradeResultType initialResultType,
        int[] retakeExamCurriculumIds,
        GradeResultType? finalResultType,
        bool isExternal,
        int modifiedBySysUserId)
    {
        var leftOuterJoin =
            from subject in this.gradeResultSubjects

            join curriculumId in retakeExamCurriculumIds.Cast<int?>()
                on subject.CurriculumId
                equals curriculumId
            into j1 from curriculumId in j1.DefaultIfEmpty()

            select (subject, curriculumId);

        var rightOuterJoin =
            from curriculumId in retakeExamCurriculumIds.Cast<int?>()

            join subject in this.gradeResultSubjects
                on curriculumId
                equals subject.CurriculumId
            into j1 from subject in j1.DefaultIfEmpty()

            select (subject, curriculumId);

        var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin).ToArray();

        bool hasNewOrRemovedCourses = fullOuterJoin.Any(c => c.subject == null || c.curriculumId == null);

        if (this.InitialResultType == initialResultType &&
            this.FinalResultType == finalResultType &&
            !hasNewOrRemovedCourses)
        {
            return;
        }

        this.InitialResultType = initialResultType;
        this.FinalResultType = finalResultType;
        this.ModifiedBySysUserId = modifiedBySysUserId;
        this.ModifyDate = DateTime.Now;

        foreach (var (subject, curriculumId) in fullOuterJoin)
        {
            if (subject == null && curriculumId != null)
            {
                this.gradeResultSubjects.Add(new GradeResultSubject(this, curriculumId.Value));
            }
            else if (subject != null && curriculumId == null)
            {
                // this should not apply for the ExtApi as they are sending
                // us the full list of subjects each time they update and
                // have no method(nor do they need one) to remove a subject's session data
                if (!isExternal && subject.IsFilled)
                {
                    throw new DomainValidationException("Cannot remove a subject that has session data filled in.");
                }

                this.gradeResultSubjects.Remove(subject);
            }
        }

        this.AssertCorrectData();
    }

    public void SetSubjectResult(
        int curriculumId,
        int? session1Grade,
        bool? session1NoShow,
        int? session2Grade,
        bool? session2NoShow,
        int? session3Grade,
        bool? session3NoShow,
        int modifiedBySysUserId)
    {
        var subject = this.gradeResultSubjects.Single(grs =>
            grs.CurriculumId == curriculumId);

        if (subject.UpdateData(
            session1Grade,
            session1NoShow,
            session2Grade,
            session2NoShow,
            session3Grade,
            session3NoShow))
        {
            this.ModifiedBySysUserId = modifiedBySysUserId;
            this.ModifyDate = DateTime.Now;
        }
    }

    private void AssertCorrectData()
    {
        if (this.InitialResultType == GradeResultType.MustRetakeExams &&
            !this.gradeResultSubjects.Any())
        {
            throw new DomainValidationException($"When the initial result is {nameof(GradeResultType.MustRetakeExams)} there should be at least one course.");
        }

        if (this.InitialResultType == GradeResultType.CompletesGrade && this.gradeResultSubjects.Any())
        {
            throw new DomainValidationException($"When the initial result is {nameof(GradeResultType.CompletesGrade)} there should be no courses.");
        }

        if (this.InitialResultType == GradeResultType.CompletesGrade && this.FinalResultType != null)
        {
            throw new DomainValidationException($"When the initial result is {nameof(GradeResultType.CompletesGrade)} there should no final result.");
        }

        if (this.FinalResultType == GradeResultType.MustRetakeExams)
        {
            throw new DomainValidationException($"The final result cannot be of type {nameof(GradeResultType.MustRetakeExams)}.");
        }
    }
}
