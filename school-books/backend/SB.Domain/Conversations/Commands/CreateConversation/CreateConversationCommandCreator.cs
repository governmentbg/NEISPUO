namespace SB.Domain;

public record CreateConversationCommandCreator
{
    public SysRole SysRoleId { get; init; }
    public int SysUserId { get; init; }
}
