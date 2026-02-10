namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class LocalAreaMapping : EntityMapping
{
    public LocalAreaMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "location";
        var tableName = "LocalArea";

        var builder = modelBuilder.Entity<LocalArea>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.LocalAreaId);
    }
}
