namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegionMapping : EntityMapping
{
    public RegionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "location";
        var tableName = "Region";

        var builder = modelBuilder.Entity<Region>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.RegionId);
    }
}
