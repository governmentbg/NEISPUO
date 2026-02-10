namespace SB.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class CustomVarValueMapping : EntityMapping
{
    public CustomVarValueMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "CustomVarValue";

        var builder = modelBuilder.Entity<CustomVarValue>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.CustomVarValueId);
    }
}
