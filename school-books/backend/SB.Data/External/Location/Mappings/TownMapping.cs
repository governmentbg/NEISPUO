namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TownMapping : EntityMapping
{
    public TownMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "location";
        var tableName = "Town";

        var builder = modelBuilder.Entity<Town>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.TownId);
    }
}
