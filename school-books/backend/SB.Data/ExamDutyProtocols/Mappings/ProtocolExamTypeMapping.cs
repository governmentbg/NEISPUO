namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ProtocolExamTypeMapping : EntityMapping
{
    public ProtocolExamTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ProtocolExamType";

        var builder = modelBuilder.Entity<ProtocolExamType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
