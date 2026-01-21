namespace SB.Domain;
public class SchoolYearSettingsClass
{
    // EF constructor
    public SchoolYearSettingsClass()
    {
        this.SchoolYearSettings = null!;
    }

    public SchoolYearSettingsClass(SchoolYearSettings schoolYearSettings, int basicClassId)
    {
        this.SchoolYearSettings = schoolYearSettings;
        this.BasicClassId = basicClassId;
    }

    public int SchoolYear { get; private set; }

    public int SchoolYearSettingsId { get; private set; }

    public int BasicClassId { get; private set; }

    // relations
    public SchoolYearSettings SchoolYearSettings { get; private set; }
}
