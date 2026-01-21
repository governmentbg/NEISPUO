namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StateExamDutyProtocolMapping : EntityMapping
{
    public StateExamDutyProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StateExamDutyProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<StateExamDutyProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.StateExamDutyProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.StateExamDutyProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Supervisors)
            .WithOne(e => e.StateExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.StateExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(StateExamDutyProtocol.Supervisors))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
