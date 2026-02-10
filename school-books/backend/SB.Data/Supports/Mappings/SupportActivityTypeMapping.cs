namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportActivityTypeMapping : EntityMapping
{
    public SupportActivityTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SupportActivityType";

        var builder = modelBuilder.Entity<SupportActivityType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
