namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamResultProtocolClassMapping : EntityMapping
{
    public ExamResultProtocolClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamResultProtocolClass";

        var builder = modelBuilder.Entity<ExamResultProtocolClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamResultProtocolId, e.ClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
