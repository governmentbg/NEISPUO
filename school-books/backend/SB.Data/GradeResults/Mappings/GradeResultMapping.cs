namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeResultMapping : EntityMapping
{
    public GradeResultMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeResult";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<GradeResult>();

        builder.ToTable(tableName, schema);

        builder.Property(e => e.GradeResultId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasKey(e => new { e.SchoolYear, e.GradeResultId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.GradeResultSubjects)
          .WithOne(e => e.GradeResult)
          .HasForeignKey(e => new { e.SchoolYear, e.GradeResultId });
        builder.Metadata
            .FindNavigation(nameof(GradeResult.GradeResultSubjects))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
