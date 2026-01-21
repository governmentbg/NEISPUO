namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookSchoolYearSettingsMapping : EntityMapping
{
    public ClassBookSchoolYearSettingsMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookSchoolYearSettings";

        var builder = modelBuilder.Entity<ClassBookSchoolYearSettings>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ClassBook))
            .WithMany()
            .HasForeignKey(nameof(ClassBookSchoolYearSettings.SchoolYear), nameof(ClassBookSchoolYearSettings.ClassBookId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(SchoolYearSettings))
            .WithMany()
            .HasForeignKey(nameof(ClassBookSchoolYearSettings.SchoolYear), nameof(ClassBookSchoolYearSettings.SchoolYearSettingsId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
