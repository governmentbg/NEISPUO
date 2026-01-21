namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookCertificateMapping : EntityMapping
{
    public RegBookCertificateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var viewName = "vwRegBookCertificate";

        var builder = modelBuilder.Entity<RegBookCertificate>();

        builder.ToView(viewName, schema);
    }
}
