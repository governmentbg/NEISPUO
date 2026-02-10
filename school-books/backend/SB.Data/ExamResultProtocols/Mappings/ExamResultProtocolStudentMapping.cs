namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamResultProtocolStudentMapping : EntityMapping
{
    public ExamResultProtocolStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamResultProtocolStudent";

        var builder = modelBuilder.Entity<ExamResultProtocolStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamResultProtocolId, e.ClassId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
