namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookPrintSignatureMapping : EntityMapping
{
    public ClassBookPrintSignatureMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookPrintSignature";

        var builder = modelBuilder.Entity<ClassBookPrintSignature>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.ClassBookPrintId, e.Index });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
