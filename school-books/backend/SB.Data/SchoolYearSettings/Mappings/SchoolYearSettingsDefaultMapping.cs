namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SchoolYearSettingsDefaultMapping : EntityMapping
{
    public SchoolYearSettingsDefaultMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SchoolYearSettingsDefault";

        var builder = modelBuilder.Entity<SchoolYearSettingsDefault>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
