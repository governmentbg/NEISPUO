namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CurriculumClassMapping : EntityMapping
{
    public CurriculumClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_year";
        var tableName = "CurriculumClass";

        var builder = modelBuilder.Entity<CurriculumClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.CurriculumId, e.ClassId });
    }
}
