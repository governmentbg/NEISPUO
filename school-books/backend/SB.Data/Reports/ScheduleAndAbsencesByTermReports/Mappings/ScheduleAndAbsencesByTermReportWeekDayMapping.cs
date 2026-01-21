namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByTermReportWeekDayMapping : EntityMapping
{
    public ScheduleAndAbsencesByTermReportWeekDayMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByTermReportWeekDay";

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByTermReportWeekDay>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByTermReportId, e.ScheduleAndAbsencesByTermReportWeekId, e.Date });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.HasMany(e => e.Hours)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByTermReportId, e.ScheduleAndAbsencesByTermReportWeekId, e.Date })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(ScheduleAndAbsencesByTermReportWeekDay.Hours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
