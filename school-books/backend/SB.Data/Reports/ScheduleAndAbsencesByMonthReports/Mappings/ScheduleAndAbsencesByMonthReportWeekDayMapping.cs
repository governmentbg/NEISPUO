namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByMonthReportWeekDayMapping : EntityMapping
{
    public ScheduleAndAbsencesByMonthReportWeekDayMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByMonthReportWeekDay";

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByMonthReportWeekDay>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId, e.ScheduleAndAbsencesByMonthReportWeekId, e.Date });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.HasMany(e => e.Hours)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId, e.ScheduleAndAbsencesByMonthReportWeekId, e.Date })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(ScheduleAndAbsencesByMonthReportWeekDay.Hours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
