namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookCertificateDuplicateBasicDocumentMapping : EntityMapping
{
    public RegBookCertificateDuplicateBasicDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookCertificateDuplicateBasicDocument";

        var builder = modelBuilder.Entity<RegBookCertificateDuplicateBasicDocument>();

        builder.ToView(tableName, schema);
    }
}
