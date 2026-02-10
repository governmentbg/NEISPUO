namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CountryMapping : EntityMapping
{
    public CountryMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "location";
        var tableName = "Country";

        var builder = modelBuilder.Entity<Country>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.CountryId);
    }
}
