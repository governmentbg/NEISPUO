namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RelocationDocumentMapping : EntityMapping
{
    public RelocationDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "RelocationDocument";

        var builder = modelBuilder.Entity<RelocationDocument>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
