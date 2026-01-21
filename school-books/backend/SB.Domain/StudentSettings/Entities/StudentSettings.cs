namespace SB.Domain;

public class StudentSettings : IAggregateRoot
{
    // EF constructor
    private StudentSettings()
    {
        this.Version = null!;
    }

    public StudentSettings(
        int personId,
        bool allowGradeEmails,
        bool allowAbsenceEmails,
        bool allowRemarkEmails,
        bool allowMessageEmails,
        bool allowGradeNotifications,
        bool allowAbsenceNotifications,
        bool allowRemarkNotifications,
        bool allowMessageNotifications)
    {
        this.PersonId = personId;
        this.AllowGradeEmails = allowGradeEmails;
        this.AllowAbsenceEmails = allowAbsenceEmails;
        this.AllowRemarkEmails = allowRemarkEmails;
        this.AllowMessageEmails = allowMessageEmails;   
        this.AllowGradeNotifications = allowGradeNotifications;
        this.AllowAbsenceNotifications = allowAbsenceNotifications;
        this.AllowRemarkNotifications = allowRemarkNotifications;
        this.AllowMessageNotifications = allowMessageNotifications;
        this.Version = null!;
    }

    public int StudentSettingsId { get; private set; }

    public int PersonId { get; private set; }

    public bool AllowGradeEmails { get; private set; }

    public bool AllowAbsenceEmails { get; private set; }

    public bool AllowRemarkEmails { get; private set; }

    public bool AllowMessageEmails { get; private set; }

    public bool AllowGradeNotifications { get; private set; }

    public bool AllowAbsenceNotifications { get; private set; }

    public bool AllowRemarkNotifications { get; private set; }

    public bool AllowMessageNotifications { get; private set; }

    public byte[] Version { get; private set; }

    public void Update(
        bool allowGradeEmails,
        bool allowAbsenceEmails,
        bool allowRemarkEmails,
        bool allowMessageEmails,
        bool allowGradeNotifications,
        bool allowAbsenceNotifications,
        bool allowRemarkNotifications,
        bool allowMessageNotifications)
    {
        this.AllowGradeEmails = allowGradeEmails;
        this.AllowAbsenceEmails = allowAbsenceEmails;
        this.AllowRemarkEmails = allowRemarkEmails;
        this.AllowMessageEmails = allowMessageEmails;
        this.AllowGradeNotifications = allowGradeNotifications;
        this.AllowAbsenceNotifications = allowAbsenceNotifications;
        this.AllowRemarkNotifications = allowRemarkNotifications;
        this.AllowMessageNotifications = allowMessageNotifications;
    }
}
