namespace SB.Domain;

public class StudentsAtRiskOfDroppingOutReportItem
{
    // EF constructor
    public StudentsAtRiskOfDroppingOutReportItem()
    {
        this.PersonalId = null!;
        this.FirstName = null!;
        this.MiddleName = null!;
        this.LastName = null!;
        this.ClassBookName = null!;
    }

    public StudentsAtRiskOfDroppingOutReportItem(
        int personId,
        string personalId,
        string firstName,
        string middleName,
        string lastName,
        string classBookName,
        decimal? unexcusedAbsencesCount,
        int? unexcusedAbsenceDaysCount)
    {
        this.PersonId = personId;
        this.PersonalId = personalId;
        this.FirstName = firstName;
        this.MiddleName = middleName;
        this.LastName = lastName;
        this.ClassBookName = classBookName;
        this.UnexcusedAbsenceHoursCount = unexcusedAbsencesCount;
        this.UnexcusedAbsenceDaysCount = unexcusedAbsenceDaysCount;
    }

    public int SchoolYear { get; private set; }

    public int StudentsAtRiskOfDroppingOutReportId { get; private set; }

    public int StudentsAtRiskOfDroppingOutReportItemId { get; private set; }

    public int PersonId { get; private set; }

    public string PersonalId { get; private set; }

    public string FirstName { get; private set; }

    public string MiddleName { get; private set; }

    public string LastName { get; private set; }

    public string ClassBookName { get; private set; }

    public decimal? UnexcusedAbsenceHoursCount { get; set; }

    public int? UnexcusedAbsenceDaysCount { get; set; }
}
