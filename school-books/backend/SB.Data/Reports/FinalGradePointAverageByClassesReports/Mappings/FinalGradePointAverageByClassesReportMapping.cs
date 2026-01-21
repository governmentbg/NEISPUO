namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class FinalGradePointAverageByClassesReportMapping : EntityMapping
{
    public FinalGradePointAverageByClassesReportMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "FinalGradePointAverageByClassesReport";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<FinalGradePointAverageByClassesReport>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.FinalGradePointAverageByClassesReportId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.FinalGradePointAverageByClassesReportId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => new { e.SchoolYear, e.FinalGradePointAverageByClassesReportId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata
            .FindNavigation(nameof(FinalGradePointAverageByClassesReport.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
