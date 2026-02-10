namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExtSystemMapping : EntityMapping
{
    public ExtSystemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "ExtSystem";

        var builder = modelBuilder.Entity<ExtSystem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.ExtSystemId);
    }
}
