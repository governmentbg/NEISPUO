namespace SB.Domain;

public interface IAuditedCommand
{
    int? SchoolYear { get; }

    int? InstId { get; }

    string ObjectName { get; }

    int? ObjectId { get; }

    int? PersonId => null;
}
