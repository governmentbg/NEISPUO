namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class NvoExamDutyProtocolStudentMapping : EntityMapping
{
    public NvoExamDutyProtocolStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "NvoExamDutyProtocolStudent";

        var builder = modelBuilder.Entity<NvoExamDutyProtocolStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.NvoExamDutyProtocolId, e.ClassId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
