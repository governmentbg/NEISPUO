namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class OffDayMapping : EntityMapping
{
    public OffDayMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "OffDay";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<OffDay>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.OffDayId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OffDayId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Classes)
            .WithOne(e => e.OffDay)
            .HasForeignKey(e => new { e.SchoolYear, e.OffDayId });
        builder.Metadata
            .FindNavigation(nameof(OffDay.Classes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.ClassBooks)
            .WithOne(e => e.OffDay)
            .HasForeignKey(e => new { e.SchoolYear, e.OffDayId });
        builder.Metadata
            .FindNavigation(nameof(OffDay.ClassBooks))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
