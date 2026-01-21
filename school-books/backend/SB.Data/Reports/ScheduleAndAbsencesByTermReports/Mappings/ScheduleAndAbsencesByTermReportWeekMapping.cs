namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByTermReportWeekMapping : EntityMapping
{
    public ScheduleAndAbsencesByTermReportWeekMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByTermReportWeek";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByTermReportWeek>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByTermReportId, e.ScheduleAndAbsencesByTermReportWeekId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ScheduleAndAbsencesByTermReportWeekId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Days)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByTermReportId, e.ScheduleAndAbsencesByTermReportWeekId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(ScheduleAndAbsencesByTermReportWeek.Days))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
