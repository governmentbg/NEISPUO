namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamDutyProtocolClassMapping : EntityMapping
{
    public ExamDutyProtocolClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamDutyProtocolClass";

        var builder = modelBuilder.Entity<ExamDutyProtocolClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamDutyProtocolId, e.ClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
