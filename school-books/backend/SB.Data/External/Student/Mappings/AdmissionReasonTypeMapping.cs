namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AdmissionReasonTypeMapping : EntityMapping
{
    public AdmissionReasonTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "AdmissionReasonType";

        var builder = modelBuilder.Entity<AdmissionReasonType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
