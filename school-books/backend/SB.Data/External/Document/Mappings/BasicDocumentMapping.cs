namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class BasicDocumentMapping : EntityMapping
{
    public BasicDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "document";
        var tableName = "BasicDocument";

        var builder = modelBuilder.Entity<BasicDocument>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
