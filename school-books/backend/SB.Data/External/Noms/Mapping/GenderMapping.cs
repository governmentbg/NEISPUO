namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GenderMapping : EntityMapping
{
    public GenderMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "noms";
        var tableName = "Gender";

        var builder = modelBuilder.Entity<Gender>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.GenderId);
    }
}
