namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CurriculumMapping : EntityMapping
{
    public CurriculumMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_year";
        var tableName = "Curriculum";

        var builder = modelBuilder.Entity<Curriculum>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.CurriculumId });

        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.Property(e => e.CurriculumGroupNum).HasColumnType("SMALLINT");
    }
}
