namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SpbsBookRecordMapping : EntityMapping
{
    public SpbsBookRecordMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SpbsBookRecord";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<SpbsBookRecord>();

        builder.ToTable(tableName, schema);
        builder.HasKey(e => new { e.SchoolYear, e.SpbsBookRecordId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.SpbsBookRecordId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Movements)
            .WithOne(e => e.SpbsBookRecord)
            .HasForeignKey(e => new { e.SchoolYear, e.SpbsBookRecordId });
        builder.Metadata
            .FindNavigation(nameof(SpbsBookRecord.Movements))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Escapes)
            .WithOne(e => e.SpbsBookRecord)
            .HasForeignKey(e => new { e.SchoolYear, e.SpbsBookRecordId });
        builder.Metadata
            .FindNavigation(nameof(SpbsBookRecord.Escapes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Absences)
            .WithOne(e => e.SpbsBookRecord)
            .HasForeignKey(e => new { e.SchoolYear, e.SpbsBookRecordId });
        builder.Metadata
            .FindNavigation(nameof(SpbsBookRecord.Absences))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
