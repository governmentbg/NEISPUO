namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SubjectTypeMapping : EntityMapping
{
    public SubjectTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "SubjectType";

        var builder = modelBuilder.Entity<SubjectType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.SubjectTypeId);
    }
}
