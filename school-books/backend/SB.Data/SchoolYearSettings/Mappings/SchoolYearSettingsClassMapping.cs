namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SchoolYearSettingsClassMapping : EntityMapping
{
    public SchoolYearSettingsClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SchoolYearSettingsClass";

        var builder = modelBuilder.Entity<SchoolYearSettingsClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SchoolYearSettingsId, e.BasicClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
