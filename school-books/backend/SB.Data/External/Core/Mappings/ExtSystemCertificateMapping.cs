namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExtSystemCertificateMapping : EntityMapping
{
    public ExtSystemCertificateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "ExtSystemCertificate";

        var builder = modelBuilder.Entity<ExtSystemCertificate>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Thumbprint);
    }
}
