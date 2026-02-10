namespace SB.Domain;

public class SchoolYearSettingsClassBook
{
    // EF constructor
    public SchoolYearSettingsClassBook()
    {
        this.SchoolYearSettings = null!;
    }

    public SchoolYearSettingsClassBook(SchoolYearSettings schoolYearSettings, int classBookId)
    {
        this.SchoolYearSettings = schoolYearSettings;
        this.ClassBookId = classBookId;
    }

    public int SchoolYear { get; private set; }

    public int SchoolYearSettingsId { get; private set; }

    public int ClassBookId { get; private set; }

    // relations
    public SchoolYearSettings SchoolYearSettings { get; private set; }
}
