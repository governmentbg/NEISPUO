namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SPPOOProfessionMapping : EntityMapping
{
    public SPPOOProfessionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "SPPOOProfession";

        var builder = modelBuilder.Entity<SPPOOProfession>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.SPPOOProfessionId);
    }
}
