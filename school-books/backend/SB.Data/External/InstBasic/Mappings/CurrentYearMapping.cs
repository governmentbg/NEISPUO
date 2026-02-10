namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CurrentYearMapping : EntityMapping
{
    public CurrentYearMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_basic";
        var tableName = "CurrentYear";

        var builder = modelBuilder.Entity<CurrentYear>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.CurrentYearId);

        builder.Property(e => e.CurrentYearId).HasColumnType("SMALLINT");
    }
}
