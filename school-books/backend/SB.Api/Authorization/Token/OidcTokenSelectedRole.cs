namespace SB.Api;

using System.Text;
using System.Text.Json.Serialization;
using Domain;

public sealed record OidcTokenSelectedRole
{
    [JsonPropertyName("Username")]
    public required string Username { get; init; }

    [JsonPropertyName("SysUserID")]
    public int SysUserId { get; init; }

    [JsonPropertyName("SysRoleID")]
    public SysRole SysRoleId { get; init; }

    [JsonPropertyName("InstitutionID")]
    public int? InstitutionId { get; init; }

    [JsonPropertyName("PositionID")]
    public int? PositionId { get; init; }

    [JsonPropertyName("MunicipalityID")]
    public int? MunicipalityId { get; init; }

    [JsonPropertyName("RegionID")]
    public int? RegionId { get; init; }

    [JsonPropertyName("BudgetingInstitutionID")]
    public int? BudgetingInstitutionId { get; init; }

    [JsonPropertyName("PersonID")]
    public int? PersonId { get; init; }

    [JsonPropertyName("StudentPersonIDs")]
    public int[]? StudentPersonIds { get; init; }

    [JsonPropertyName("ClassBookIDs")]
    public int[]? ClassBookIds { get; init; }

    #pragma warning disable IDE0051 // Private member 'OidcTokenSelectedRole.PrintMembers' is unused
    private bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"Username = {this.Username}, ");
        stringBuilder.Append($"SysUserId = {this.SysUserId}, ");
        stringBuilder.Append($"SysRoleId = {this.SysRoleId}, ");
        stringBuilder.Append($"InstitutionId = {this.InstitutionId}, ");
        stringBuilder.Append($"PositionId = {this.PositionId}, ");
        stringBuilder.Append($"MunicipalityId = {this.MunicipalityId}, ");
        stringBuilder.Append($"RegionId = {this.RegionId}, ");
        stringBuilder.Append($"BudgetingInstitutionId = {this.BudgetingInstitutionId}, ");
        stringBuilder.Append($"PersonId = {this.PersonId}, ");
        stringBuilder.Append($"StudentPersonIds = {(this.StudentPersonIds == null ? "" : "[" + string.Join(", ", this.StudentPersonIds) + "]")}, ");
        stringBuilder.Append($"ClassBookIds = {(this.ClassBookIds == null ? "" : "[" + string.Join(", ", this.ClassBookIds) + "]")}");
        return true;
    }
    #pragma warning restore IDE0051
}
