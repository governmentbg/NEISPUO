namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StudentsAtRiskOfDroppingOutReportItemMapping : EntityMapping
{
    public StudentsAtRiskOfDroppingOutReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StudentsAtRiskOfDroppingOutReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<StudentsAtRiskOfDroppingOutReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.StudentsAtRiskOfDroppingOutReportId, e.StudentsAtRiskOfDroppingOutReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.StudentsAtRiskOfDroppingOutReportItemId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
