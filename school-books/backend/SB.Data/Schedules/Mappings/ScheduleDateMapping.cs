namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleDateMapping : EntityMapping
{
    public ScheduleDateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleDate";

        var builder = modelBuilder.Entity<ScheduleDate>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleId, e.Date });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
