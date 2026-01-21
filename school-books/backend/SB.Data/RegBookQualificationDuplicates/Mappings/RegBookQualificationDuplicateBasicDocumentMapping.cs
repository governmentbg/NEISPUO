namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookQualificationDuplicateBasicDocumentMapping : EntityMapping
{
    public RegBookQualificationDuplicateBasicDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookQualificationDuplicateBasicDocument";

        var builder = modelBuilder.Entity<RegBookQualificationDuplicateBasicDocument>();

        builder.ToView(tableName, schema);
    }
}
