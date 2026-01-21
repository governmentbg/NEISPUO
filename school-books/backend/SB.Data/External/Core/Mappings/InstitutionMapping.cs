namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class InstitutionMapping : EntityMapping
{
    public InstitutionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "Institution";

        var builder = modelBuilder.Entity<Institution>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.InstitutionId);
    }
}
