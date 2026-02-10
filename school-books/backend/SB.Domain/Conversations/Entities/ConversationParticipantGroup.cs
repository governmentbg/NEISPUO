namespace SB.Domain;

using SB.Data;

public class ConversationParticipantGroup
{
    // Constructor for EF
    private ConversationParticipantGroup()
    {
        this.GroupName = null!;
    }

    public ConversationParticipantGroup(
        int schoolYear,
        string groupName,
        ParticipantType participantType,
        int? classBookId)
    {
        this.SchoolYear = schoolYear;
        this.GroupName = groupName;
        this.ParticipantType = participantType;
        this.ClassBookId = classBookId;
    }

    public int SchoolYear { get; private set; }
    public int ConversationParticipantGroupId { get; private set; }
    public string GroupName { get; private set; }
    public ParticipantType ParticipantType { get; set; }
    public int? ClassBookId { get; set; }
}
