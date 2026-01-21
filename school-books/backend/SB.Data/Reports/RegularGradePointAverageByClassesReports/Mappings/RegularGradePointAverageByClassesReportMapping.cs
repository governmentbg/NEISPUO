namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegularGradePointAverageByClassesReportMapping : EntityMapping
{
    public RegularGradePointAverageByClassesReportMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "RegularGradePointAverageByClassesReport";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<RegularGradePointAverageByClassesReport>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.RegularGradePointAverageByClassesReportId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.RegularGradePointAverageByClassesReportId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.RegularGradePointAverageByClassesReportId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(RegularGradePointAverageByClassesReport.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
