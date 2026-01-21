namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamsReportItemMapping : EntityMapping
{
    public ExamsReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamsReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ExamsReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamsReportId, e.ExamsReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ExamsReportItemId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
