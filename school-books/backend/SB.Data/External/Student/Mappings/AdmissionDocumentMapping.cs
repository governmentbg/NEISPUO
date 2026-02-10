namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AdmissionDocumentMapping : EntityMapping
{
    public AdmissionDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "AdmissionDocument";

        var builder = modelBuilder.Entity<AdmissionDocument>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
