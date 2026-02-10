namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class OffDayClassBookMapping : EntityMapping
{
    public OffDayClassBookMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "OffDayClassBook";

        var builder = modelBuilder.Entity<OffDayClassBook>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.OffDayId, e.ClassBookId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
