namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class InstitutionConfDataMapping : EntityMapping
{
    public InstitutionConfDataMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "InstitutionConfData";

        var builder = modelBuilder.Entity<InstitutionConfData>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.InstitutionConfDataID);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
