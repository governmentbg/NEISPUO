namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookCertificateBasicDocumentMapping : EntityMapping
{
    public RegBookCertificateBasicDocumentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var viewName = "vwRegBookCertificateBasicDocument";

        var builder = modelBuilder.Entity<RegBookCertificateBasicDocument>();

        builder.ToView(viewName, schema);
    }
}
