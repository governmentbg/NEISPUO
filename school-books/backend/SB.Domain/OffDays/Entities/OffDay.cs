namespace SB.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

public class OffDay : IAggregateRoot
{
    // EF constructor
    private OffDay()
    {
        this.Description = null!;
    }

    public OffDay(
        int schoolYear,
        int instId,
        DateTime from,
        DateTime to,
        string description,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        bool isPgOffProgramDay,
        int createdBySysUserId)
    {
        if (from > to)
        {
            throw new DomainValidationException("End date should be later than start date");
        }

        if (isForAllClasses && (basicClassIds.Any() || classBookIds.Any()))
        {
            throw new DomainValidationException("When IsForAllClasses=True BasicClassIds and ClassBookIds should be empty.");
        }

        if (basicClassIds.Any() && classBookIds.Any())
        {
            throw new DomainValidationException("Cant use both BasicClassIds and ClassBookIds in an OffDay.");
        }

        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.From = from;
        this.To = to;
        this.Description = description;
        this.IsPgOffProgramDay = isPgOffProgramDay;
        this.IsForAllClasses = isForAllClasses;

        this.UpdateClasses(basicClassIds);
        this.UpdateClassBooks(classBookIds);

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int OffDayId { get; private set; }

    public int InstId { get; private set; }

    public DateTime From { get; private set; }

    public DateTime To { get; private set; }

    public string Description { get; private set; }

    public bool IsPgOffProgramDay { get; private set; }

    public bool IsForAllClasses { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    private readonly List<OffDayClass> classes = new();
    public IReadOnlyCollection<OffDayClass> Classes => this.classes.AsReadOnly();

    private readonly List<OffDayClassBook> classBooks = new();
    public IReadOnlyCollection<OffDayClassBook> ClassBooks => this.classBooks.AsReadOnly();

    public void UpdateData(
        DateTime from,
        DateTime to,
        string description,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        bool isPgOffProgramDay,
        int modifiedBySysUserId)
    {
        if (from > to)
        {
            throw new DomainValidationException("End date should be later than start date");
        }

        if (isForAllClasses && (basicClassIds.Any() || classBookIds.Any()))
        {
            throw new DomainValidationException("When IsForAllClasses=True BasicClassIds and ClassBookIds should be empty.");
        }

        if (basicClassIds.Any() && classBookIds.Any())
        {
            throw new DomainValidationException("Cant use both BasicClassIds and ClassBookIds in an OffDay.");
        }

        this.From = from;
        this.To = to;
        this.Description = description;
        this.IsPgOffProgramDay = isPgOffProgramDay;
        this.IsForAllClasses = isForAllClasses;

        this.UpdateClasses(basicClassIds);
        this.UpdateClassBooks(classBookIds);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void RemoveClassBookId(int classBookId)
    {
        this.classBooks.RemoveAll(cb => cb.ClassBookId == classBookId);
    }

    public void AddOffDayToClassBook(int classBookId)
    {
        this.classBooks.Add(new OffDayClassBook(this, classBookId));
    }

    private void UpdateClasses(int[] basicClassIds)
    {
        this.classes.Clear();
        this.classes.AddRange(basicClassIds.Select(bClassId => new OffDayClass(this, bClassId)));
    }

    private void UpdateClassBooks(int[] classBookIds)
    {
        this.classBooks.Clear();
        this.classBooks.AddRange(classBookIds.Select(classBookId => new OffDayClassBook(this, classBookId)));
    }
}
