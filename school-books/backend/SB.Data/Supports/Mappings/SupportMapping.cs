namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportMapping : EntityMapping
{
    public SupportMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Support";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Support>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SupportId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.SupportId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Activities)
           .WithOne(e => e.Support)
           .HasForeignKey(e => new { e.SchoolYear, e.SupportId });
        builder.Metadata
            .FindNavigation(nameof(Support.Activities))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.DifficultyTypes)
          .WithOne(e => e.Support)
          .HasForeignKey(e => new { e.SchoolYear, e.SupportId });
        builder.Metadata
            .FindNavigation(nameof(Support.DifficultyTypes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Teachers)
          .WithOne(e => e.Support)
          .HasForeignKey(e => new { e.SchoolYear, e.SupportId });
        builder.Metadata
            .FindNavigation(nameof(Support.Teachers))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
          .WithOne(e => e.Support)
          .HasForeignKey(e => new { e.SchoolYear, e.SupportId });
        builder.Metadata
            .FindNavigation(nameof(Support.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
