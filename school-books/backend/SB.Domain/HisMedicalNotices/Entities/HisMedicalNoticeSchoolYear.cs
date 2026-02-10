namespace SB.Domain;

public class HisMedicalNoticeSchoolYear
{
    // EF constructor
    private HisMedicalNoticeSchoolYear()
    {
        this.HisMedicalNotice = null!;
    }

    public HisMedicalNoticeSchoolYear(
        HisMedicalNotice hisMedicalNotice,
        int schoolYear)
    {
        this.HisMedicalNotice = hisMedicalNotice;
        this.SchoolYear = schoolYear;
    }

    public int HisMedicalNoticeId { get; private set; }

    public int SchoolYear { get; private set; }

    // relations
    public HisMedicalNotice HisMedicalNotice { get; private set; }
}
