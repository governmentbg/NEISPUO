namespace SB.Domain;

using System;

public class PersonMedicalNotice
{
    // EF constructor
    private PersonMedicalNotice()
    {
    }

    public PersonMedicalNotice(
        int schoolYear,
        int personId,
        int hisMedicalNoticeId)
    {
        this.SchoolYear = schoolYear;
        this.PersonId = personId;
        this.HisMedicalNoticeId = hisMedicalNoticeId;

        this.CreateDate = DateTime.Now;
    }

    public int SchoolYear { get; private set; }

    public int PersonId { get; private set; }

    public int HisMedicalNoticeId { get; private set; }

    public DateTime CreateDate { get; private set; }
}
