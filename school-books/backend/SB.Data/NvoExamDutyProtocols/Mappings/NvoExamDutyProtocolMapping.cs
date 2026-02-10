namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class NvoExamDutyProtocolMapping : EntityMapping
{
    public NvoExamDutyProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "NvoExamDutyProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<NvoExamDutyProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.NvoExamDutyProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.NvoExamDutyProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Students)
            .WithOne(e => e.NvoExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.NvoExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(NvoExamDutyProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Supervisors)
            .WithOne(e => e.NvoExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.NvoExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(NvoExamDutyProtocol.Supervisors))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
