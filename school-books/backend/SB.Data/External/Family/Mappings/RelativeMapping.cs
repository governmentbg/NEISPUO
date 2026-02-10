namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RelativeMapping : EntityMapping
{
    public RelativeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "family";
        var tableName = "Relative";

        var builder = modelBuilder.Entity<Relative>();

        builder.ToTable(tableName, schema);

        builder.HasKey(p => p.RelativeId);
    }
}
