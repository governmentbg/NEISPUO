namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class OffDayClassMapping : EntityMapping
{
    public OffDayClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "OffDayClass";

        var builder = modelBuilder.Entity<OffDayClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.OffDayId, e.BasicClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
