namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportActivityMapping : EntityMapping
{
    public SupportActivityMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SupportActivity";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<SupportActivity>();

        builder.ToTable(tableName, schema);

        builder.Property(e => e.SupportActivityId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasKey(e => new { e.SchoolYear, e.SupportId, e.SupportActivityId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
