namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SkillsCheckExamDutyProtocolSupervisorMapping : EntityMapping
{
    public SkillsCheckExamDutyProtocolSupervisorMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SkillsCheckExamDutyProtocolSupervisor";

        var builder = modelBuilder.Entity<SkillsCheckExamDutyProtocolSupervisor>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SkillsCheckExamDutyProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
