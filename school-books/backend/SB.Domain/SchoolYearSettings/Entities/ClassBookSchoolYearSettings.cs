namespace SB.Domain;

using System;

public class ClassBookSchoolYearSettings : IAggregateRoot
{
    // EF constructor
    private ClassBookSchoolYearSettings()
    {
    }

    public ClassBookSchoolYearSettings(
        int schoolYear,
        int classBookId,
        int? schoolYearSettingsId,
        DateTime schoolYearStartDateLimit,
        DateTime schoolYearStartDate,
        DateTime firstTermEndDate,
        DateTime secondTermStartDate,
        DateTime schoolYearEndDate,
        DateTime schoolYearEndDateLimit,
        bool hasFutureEntryLock,
        int? pastMonthLockDay)
    {
        if (schoolYearStartDateLimit > schoolYearStartDate ||
            schoolYearStartDate > firstTermEndDate ||
            firstTermEndDate > secondTermStartDate ||
            secondTermStartDate > schoolYearEndDate ||
            schoolYearEndDate > schoolYearEndDateLimit)
        {
            throw new DomainValidationException($"incorrect dates");
        }

        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.SchoolYearSettingsId = schoolYearSettingsId;
        this.SchoolYearStartDateLimit = schoolYearStartDateLimit;
        this.SchoolYearStartDate = schoolYearStartDate;
        this.FirstTermEndDate = firstTermEndDate;
        this.SecondTermStartDate = secondTermStartDate;
        this.SchoolYearEndDate = schoolYearEndDate;
        this.SchoolYearEndDateLimit = schoolYearEndDateLimit;
        this.HasFutureEntryLock = hasFutureEntryLock;
        this.PastMonthLockDay = pastMonthLockDay;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int? SchoolYearSettingsId { get; private set; }

    public DateTime SchoolYearStartDateLimit { get; private set; }

    public DateTime SchoolYearStartDate { get; private set; }

    public DateTime FirstTermEndDate { get; private set; }

    public DateTime SecondTermStartDate { get; private set; }

    public DateTime SchoolYearEndDate { get; private set; }

    public DateTime SchoolYearEndDateLimit { get; private set; }

    public bool HasFutureEntryLock { get; private set;}

    public int? PastMonthLockDay { get; private set;}

    public byte[] Version { get; private set; } = null!;

    public void UpdateData(
        int? schoolYearSettingsId,
        DateTime schoolYearStartDateLimit,
        DateTime schoolYearStartDate,
        DateTime firstTermEndDate,
        DateTime secondTermStartDate,
        DateTime schoolYearEndDate,
        DateTime schoolYearEndDateLimit,
        bool HasFutureEntryLock,
        int? PastMonthLockDay)
    {
        if (schoolYearStartDateLimit > schoolYearStartDate ||
            schoolYearStartDate > firstTermEndDate ||
            firstTermEndDate > secondTermStartDate ||
            secondTermStartDate > schoolYearEndDate ||
            schoolYearEndDate > schoolYearEndDateLimit)
        {
            throw new DomainValidationException($"incorrect dates");
        }

        this.SchoolYearSettingsId = schoolYearSettingsId;
        this.SchoolYearStartDateLimit = schoolYearStartDateLimit;
        this.SchoolYearStartDate = schoolYearStartDate;
        this.FirstTermEndDate = firstTermEndDate;
        this.SecondTermStartDate = secondTermStartDate;
        this.SchoolYearEndDate = schoolYearEndDate;
        this.SchoolYearEndDateLimit = schoolYearEndDateLimit;
        this.HasFutureEntryLock = HasFutureEntryLock;
        this.PastMonthLockDay = PastMonthLockDay;
    }
}
