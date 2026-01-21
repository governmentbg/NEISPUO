namespace SB.Domain;

using System.Text.Json.Serialization;

public record SeparateClassBooksExtApiCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? ParentClassId { get; init; }

    public SeparateClassBooksExtApiCommandClassBook[]? ChildClassBooks { get; init; }

    [JsonIgnore] public string ObjectName => nameof(ClassBook);
    [JsonIgnore] public virtual int? ObjectId => null;
}
