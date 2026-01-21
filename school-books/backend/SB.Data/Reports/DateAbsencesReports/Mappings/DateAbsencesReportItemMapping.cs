namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class DateAbsencesReportItemMapping : EntityMapping
{
    public DateAbsencesReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "DateAbsencesReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<DateAbsencesReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.DateAbsencesReportId, e.DateAbsencesReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.DateAbsencesReportItemId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
