namespace SB.Domain;

public interface IRequestContext
{
    int AuditModuleId { get; }

    string RequestId { get; }
    string RemoteIpAddress { get; }
    string UserAgent => string.Empty;

    bool IsAuthenticated { get; }
    int? SysUserId { get; }
    int? SysRoleId => null;
    string? LoginSessionId => null;
    string? Username => null;
    string? FirstName => null;
    string? MiddleName => null;
    string? LastName => null;
}
