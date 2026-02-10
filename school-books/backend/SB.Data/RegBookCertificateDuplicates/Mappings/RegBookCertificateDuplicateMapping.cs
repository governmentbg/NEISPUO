namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class RegBookCertificateDuplicateMapping : EntityMapping
{
    public RegBookCertificateDuplicateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "vwRegBookCertificateDuplicate";

        var builder = modelBuilder.Entity<RegBookCertificateDuplicate>();

        builder.ToView(tableName, schema);
    }
}
