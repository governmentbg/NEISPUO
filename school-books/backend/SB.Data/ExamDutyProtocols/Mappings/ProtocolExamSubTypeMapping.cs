namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ProtocolExamSubTypeMapping : EntityMapping
{
    public ProtocolExamSubTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ProtocolExamSubType";

        var builder = modelBuilder.Entity<ProtocolExamSubType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
