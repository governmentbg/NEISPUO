namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookQualificationDuplicateMapping : EntityMapping
{
    public RegBookQualificationDuplicateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookQualificationDuplicate";

        var builder = modelBuilder.Entity<RegBookQualificationDuplicate>();

        builder.ToView(tableName, schema);
    }
}
