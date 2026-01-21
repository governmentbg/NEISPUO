namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SchoolYearSettingsMapping : EntityMapping
{
    public SchoolYearSettingsMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SchoolYearSettings";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<SchoolYearSettings>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SchoolYearSettingsId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.SchoolYearSettingsId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Classes)
            .WithOne(e => e.SchoolYearSettings)
            .HasForeignKey(e => new { e.SchoolYear, e.SchoolYearSettingsId });
        builder.Metadata
            .FindNavigation(nameof(SchoolYearSettings.Classes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.ClassBooks)
            .WithOne(e => e.SchoolYearSettings)
            .HasForeignKey(e => new { e.SchoolYear, e.SchoolYearSettingsId });
        builder.Metadata
            .FindNavigation(nameof(SchoolYearSettings.ClassBooks))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
