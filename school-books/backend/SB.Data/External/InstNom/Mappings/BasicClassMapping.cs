namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class BasicClassMapping : EntityMapping
{
    public BasicClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "BasicClass";

        var builder = modelBuilder.Entity<BasicClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.BasicClassId);
    }
}
