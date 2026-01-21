namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AbsencesByStudentsReportItemMapping : EntityMapping
{
    public AbsencesByStudentsReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "AbsencesByStudentsReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<AbsencesByStudentsReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.AbsencesByStudentsReportId, e.AbsencesByStudentsReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.AbsencesByStudentsReportItemId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
