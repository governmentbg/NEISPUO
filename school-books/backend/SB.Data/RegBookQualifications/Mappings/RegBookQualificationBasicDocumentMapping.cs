namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookQualificationBasicDocumentMapping : EntityMapping
{
    public RegBookQualificationBasicDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookQualificationBasicDocument";

        var builder = modelBuilder.Entity<RegBookQualificationBasicDocument>();

        builder.ToView(tableName, schema);
    }
}
