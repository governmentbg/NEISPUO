namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByTermReportWeekDayHourMapping : EntityMapping
{
    public ScheduleAndAbsencesByTermReportWeekDayHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByTermReportWeekDayHour";

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByTermReportWeekDayHour>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByTermReportId, e.ScheduleAndAbsencesByTermReportWeekId, e.Date, e.HourNumber });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
