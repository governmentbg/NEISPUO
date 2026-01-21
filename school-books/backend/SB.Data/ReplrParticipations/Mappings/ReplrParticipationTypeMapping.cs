namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ReplrParticipationTypeMapping : EntityMapping
{
    public ReplrParticipationTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ReplrParticipationType";

        var builder = modelBuilder.Entity<ReplrParticipationType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
