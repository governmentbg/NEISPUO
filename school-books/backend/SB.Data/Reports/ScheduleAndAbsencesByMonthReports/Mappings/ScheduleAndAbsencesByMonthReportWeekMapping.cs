namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByMonthReportWeekMapping : EntityMapping
{
    public ScheduleAndAbsencesByMonthReportWeekMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByMonthReportWeek";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByMonthReportWeek>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId, e.ScheduleAndAbsencesByMonthReportWeekId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ScheduleAndAbsencesByMonthReportWeekId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Days)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId, e.ScheduleAndAbsencesByMonthReportWeekId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(ScheduleAndAbsencesByMonthReportWeek.Days))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
