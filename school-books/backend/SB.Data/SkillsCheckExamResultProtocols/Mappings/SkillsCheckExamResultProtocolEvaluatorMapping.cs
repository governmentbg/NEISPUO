namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SkillsCheckExamResultProtocolEvaluatorMapping : EntityMapping
{
    public SkillsCheckExamResultProtocolEvaluatorMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SkillsCheckExamResultProtocolEvaluator";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<SkillsCheckExamResultProtocolEvaluator>();

        builder.ToTable(tableName, schema);

        builder.Property(e => e.SkillsCheckExamResultProtocolEvaluatorId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasKey(e => new { e.SchoolYear, e.SkillsCheckExamResultProtocolId, e.SkillsCheckExamResultProtocolEvaluatorId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
