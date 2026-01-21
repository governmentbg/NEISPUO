namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleAndAbsencesByMonthReportMapping : EntityMapping
{
    public ScheduleAndAbsencesByMonthReportMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleAndAbsencesByMonthReport";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ScheduleAndAbsencesByMonthReport>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ScheduleAndAbsencesByMonthReportId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Weeks)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleAndAbsencesByMonthReportId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(ScheduleAndAbsencesByMonthReport.Weeks))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
