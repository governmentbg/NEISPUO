namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CurriculumStudentMapping : EntityMapping
{
    public CurriculumStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_year";
        var tableName = "CurriculumStudent";

        var builder = modelBuilder.Entity<CurriculumStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.CurriculumId, e.PersonId });
    }
}
