namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class SchoolYearSettings : IAggregateRoot
{
    // EF constructor
    private SchoolYearSettings()
    {
        this.Description = null!;
    }

    public SchoolYearSettings(
        int schoolYear,
        int instId,
        DateTime? schoolYearStartDate,
        DateTime? firstTermEndDate,
        DateTime? secondTermStartDate,
        DateTime? schoolYearEndDate,
        string description,
        bool hasFutureEntryLock,
        int? pastMonthLockDay,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        int createdBySysUserId)
    {
        if (schoolYearStartDate > firstTermEndDate ||
            firstTermEndDate > secondTermStartDate ||
            secondTermStartDate > schoolYearEndDate)
        {
            throw new DomainValidationException($"incorrect dates");
        }

        if (isForAllClasses && (basicClassIds.Any() || classBookIds.Any()))
        {
            throw new DomainValidationException("When IsForAllClasses=True BasicClassIds and ClassBookIds should be empty.");
        }

        if (basicClassIds.Any() && classBookIds.Any())
        {
            throw new DomainValidationException("Cant use both BasicClassIds and ClassBookIds in an SchoolYearSettings.");
        }

        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.SchoolYearStartDate = schoolYearStartDate;
        this.FirstTermEndDate = firstTermEndDate;
        this.SecondTermStartDate = secondTermStartDate;
        this.SchoolYearEndDate = schoolYearEndDate;
        this.Description = description;
        this.HasFutureEntryLock = hasFutureEntryLock;
        this.PastMonthLockDay = pastMonthLockDay;
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

    public int SchoolYearSettingsId { get; private set; }

    public int InstId { get; private set; }

    public DateTime? SchoolYearStartDate { get; private set; }

    public DateTime? FirstTermEndDate { get; private set; }

    public DateTime? SecondTermStartDate { get; private set; }

    public DateTime? SchoolYearEndDate { get; private set; }

    public string Description { get; private set; }

    public bool HasFutureEntryLock { get; private set; }

    public int? PastMonthLockDay { get; private set; }

    public bool IsForAllClasses { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    private readonly List<SchoolYearSettingsClass> classes = new();
    public IReadOnlyCollection<SchoolYearSettingsClass> Classes => this.classes.AsReadOnly();

    private readonly List<SchoolYearSettingsClassBook> classBooks = new();
    public IReadOnlyCollection<SchoolYearSettingsClassBook> ClassBooks => this.classBooks.AsReadOnly();

    public void UpdateData(
        DateTime? schoolYearStartDate,
        DateTime? firstTermEndDate,
        DateTime? secondTermStartDate,
        DateTime? schoolYearEndDate,
        string description,
        bool hasFutureEntryLock,
        int? pastMonthLockDay,
        bool isForAllClasses,
        int[] basicClassIds,
        int[] classBookIds,
        int modifiedBySysUserId)
    {
        if (schoolYearStartDate > firstTermEndDate ||
            firstTermEndDate > secondTermStartDate ||
            secondTermStartDate > schoolYearEndDate)
        {
            throw new DomainValidationException($"incorrect dates");
        }

        if (isForAllClasses && (basicClassIds.Any() || classBookIds.Any()))
        {
            throw new DomainValidationException("When IsForAllClasses=True BasicClassIds and ClassBookIds should be empty.");
        }

        if (basicClassIds.Any() && classBookIds.Any())
        {
            throw new DomainValidationException("Cant use both BasicClassIds and ClassBookIds in an SchoolYearSettings.");
        }

        this.SchoolYearStartDate = schoolYearStartDate;
        this.FirstTermEndDate = firstTermEndDate;
        this.SecondTermStartDate = secondTermStartDate;
        this.SchoolYearEndDate = schoolYearEndDate;
        this.Description = description;
        this.HasFutureEntryLock = hasFutureEntryLock;
        this.PastMonthLockDay = pastMonthLockDay;
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

    public void AddClassBookId(int classBookId)
    {
        this.classBooks.Add(new SchoolYearSettingsClassBook(this, classBookId));
    }

    private void UpdateClasses(int[] basicClassIds)
    {
        this.classes.Clear();
        this.classes.AddRange(basicClassIds.Select(bClassId => new SchoolYearSettingsClass(this, bClassId)));
    }

    private void UpdateClassBooks(int[] classBookIds)
    {
        this.classBooks.Clear();
        this.classBooks.AddRange(classBookIds.Select(classBookId => new SchoolYearSettingsClassBook(this, classBookId)));
    }
}
