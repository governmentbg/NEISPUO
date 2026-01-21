namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SchoolYearSettingsClassBookMapping : EntityMapping
{
    public SchoolYearSettingsClassBookMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SchoolYearSettingsClassBook";

        var builder = modelBuilder.Entity<SchoolYearSettingsClassBook>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SchoolYearSettingsId, e.ClassBookId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
