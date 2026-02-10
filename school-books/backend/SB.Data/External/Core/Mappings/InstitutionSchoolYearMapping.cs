namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class InstitutionSchoolYearMapping : EntityMapping
{
    public InstitutionSchoolYearMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "InstitutionSchoolYear";

        var builder = modelBuilder.Entity<InstitutionSchoolYear>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.InstitutionId, e.SchoolYear });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
