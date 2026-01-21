namespace SB.Blobs;

using System.Text.Json.Serialization;

public record TokenSelectedRole
{
    [JsonPropertyName("Username")]
    public string Username { get; init; } = null!;

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

    [JsonPropertyName("studentPersonIds")]
    public int[] StudentPersonIds { get; init; }

    [JsonPropertyName("classBookIds")]
    public int[] ClassBookIds { get; init; }
}
