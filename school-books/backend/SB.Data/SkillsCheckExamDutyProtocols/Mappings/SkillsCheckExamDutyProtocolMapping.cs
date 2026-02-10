namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SkillsCheckExamDutyProtocolMapping : EntityMapping
{
    public SkillsCheckExamDutyProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SkillsCheckExamDutyProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<SkillsCheckExamDutyProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SkillsCheckExamDutyProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.SkillsCheckExamDutyProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Supervisors)
            .WithOne(e => e.SkillsCheckExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.SkillsCheckExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(SkillsCheckExamDutyProtocol.Supervisors))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
