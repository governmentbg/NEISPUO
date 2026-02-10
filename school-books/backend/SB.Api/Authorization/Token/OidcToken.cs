namespace SB.Api;

public record OidcToken(
    string Jti,
    string Exp,
    string SessionId,
    OidcTokenSelectedRole SelectedRole);
