namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByMonthReportWeekDayHourMapping : EntityMapping
{
    public ScheduleAndAbsencesByMonthReportWeekDayHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByMonthReportWeekDayHour";

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByMonthReportWeekDayHour>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId, e.ScheduleAndAbsencesByMonthReportWeekId, e.Date, e.HourNumber });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
