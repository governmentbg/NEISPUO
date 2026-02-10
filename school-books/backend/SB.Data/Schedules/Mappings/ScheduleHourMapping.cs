namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleHourMapping : EntityMapping
{
    public ScheduleHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleHour";

        var builder = modelBuilder.Entity<ScheduleHour>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleId, e.Day, e.HourNumber, e.CurriculumId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
