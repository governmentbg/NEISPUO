namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class MunicipalityMapping : EntityMapping
{
    public MunicipalityMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "location";
        var tableName = "Municipality";

        var builder = modelBuilder.Entity<Municipality>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.MunicipalityId);
    }
}
