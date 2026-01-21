namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationDegreeMapping : EntityMapping
{
    public QualificationDegreeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationDegree";

        var builder = modelBuilder.Entity<QualificationDegree>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
