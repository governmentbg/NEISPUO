namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamResultProtocolMapping : EntityMapping
{
    public ExamResultProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamResultProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ExamResultProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamResultProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ExamResultProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Classes)
            .WithOne(e => e.ExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamResultProtocol.Classes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.ExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamResultProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.ExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.ExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(ExamResultProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
