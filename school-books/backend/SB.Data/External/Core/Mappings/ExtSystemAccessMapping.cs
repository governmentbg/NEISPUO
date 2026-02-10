namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExtSystemAccessMapping : EntityMapping
{
    public ExtSystemAccessMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "ExtSystemAccess";

        var builder = modelBuilder.Entity<ExtSystemAccess>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.ExtSystemAccessId);
    }
}
