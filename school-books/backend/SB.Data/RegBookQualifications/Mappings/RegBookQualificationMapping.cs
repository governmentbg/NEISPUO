namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookQualificationMapping : EntityMapping
{
    public RegBookQualificationMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookQualification";

        var builder = modelBuilder.Entity<RegBookQualification>();

        builder.ToView(tableName, schema);
    }
}
