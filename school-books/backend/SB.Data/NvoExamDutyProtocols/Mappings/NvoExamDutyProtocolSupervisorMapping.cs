namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class NvoExamDutyProtocolSupervisorMapping : EntityMapping
{
    public NvoExamDutyProtocolSupervisorMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "NvoExamDutyProtocolSupervisor";

        var builder = modelBuilder.Entity<NvoExamDutyProtocolSupervisor>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.NvoExamDutyProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
