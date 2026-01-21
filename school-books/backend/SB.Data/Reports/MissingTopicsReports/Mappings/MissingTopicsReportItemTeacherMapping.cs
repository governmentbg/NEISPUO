namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class MissingTopicsReportItemTeacherMapping : EntityMapping
{
    public MissingTopicsReportItemTeacherMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "MissingTopicsReportItemTeacher";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<MissingTopicsReportItemTeacher>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.MissingTopicsReportId, e.MissingTopicsReportItemId, e.MissingTopicsReportItemTeacherId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.MissingTopicsReportItemTeacherId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
