namespace SB.Domain;

using Data;

public partial interface IConversationParticipantsNomRepository
{
    public record ConversationParticipantsNomVO(ConversationParticipantsNomVOParticipant Id, string Name);

    public record ConversationParticipantsNomVOParticipant(
        int? InstId,
        int? SysUserId,
        string Title,
        int? ClassBookId,
        ParticipantType ParticipantType);
}
