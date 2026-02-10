namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class MissingTopicsReportItemMapping : EntityMapping
{
    public MissingTopicsReportItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "MissingTopicsReportItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<MissingTopicsReportItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.MissingTopicsReportId, e.MissingTopicsReportItemId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.MissingTopicsReportItemId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Teachers)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.MissingTopicsReportId, e.MissingTopicsReportItemId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(MissingTopicsReportItem.Teachers))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
