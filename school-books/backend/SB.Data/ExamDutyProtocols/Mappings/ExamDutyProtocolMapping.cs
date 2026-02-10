namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamDutyProtocolMapping : EntityMapping
{
    public ExamDutyProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamDutyProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ExamDutyProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamDutyProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ExamDutyProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Classes)
            .WithOne(e => e.ExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamDutyProtocol.Classes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.ExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamDutyProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Supervisors)
            .WithOne(e => e.ExamDutyProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamDutyProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamDutyProtocol.Supervisors))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
