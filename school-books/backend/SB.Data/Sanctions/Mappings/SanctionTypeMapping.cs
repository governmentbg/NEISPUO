namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SanctionTypeMapping : EntityMapping
{
    public SanctionTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "SanctionType";

        var builder = modelBuilder.Entity<SanctionType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
