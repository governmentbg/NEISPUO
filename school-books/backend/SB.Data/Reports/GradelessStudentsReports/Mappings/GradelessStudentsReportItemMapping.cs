namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradelessStudentsReportItemMapping : EntityMapping
{
    public GradelessStudentsReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradelessStudentsReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<GradelessStudentsReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradelessStudentsReportId, e.GradelessStudentsReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.GradelessStudentsReportItemId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
