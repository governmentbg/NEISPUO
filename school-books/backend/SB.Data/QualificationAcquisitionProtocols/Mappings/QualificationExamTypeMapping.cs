namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationExamTypeMapping : EntityMapping
{
    public QualificationExamTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationExamType";

        var builder = modelBuilder.Entity<QualificationExamType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
