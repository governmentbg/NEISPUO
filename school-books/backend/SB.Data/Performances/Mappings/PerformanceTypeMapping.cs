namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PerformanceTypeMapping : EntityMapping
{
    public PerformanceTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "PerformanceType";

        var builder = modelBuilder.Entity<PerformanceType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
