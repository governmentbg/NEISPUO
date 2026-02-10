namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationExamResultProtocolClassMapping : EntityMapping
{
    public QualificationExamResultProtocolClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationExamResultProtocolClass";

        var builder = modelBuilder.Entity<QualificationExamResultProtocolClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.QualificationExamResultProtocolId, e.ClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
