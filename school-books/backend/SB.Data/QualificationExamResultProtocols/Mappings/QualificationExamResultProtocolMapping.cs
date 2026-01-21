namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationExamResultProtocolMapping : EntityMapping
{
    public QualificationExamResultProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationExamResultProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<QualificationExamResultProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.QualificationExamResultProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.QualificationExamResultProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Classes)
            .WithOne(e => e.QualificationExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.QualificationExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(QualificationExamResultProtocol.Classes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.QualificationExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.QualificationExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(QualificationExamResultProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.QualificationExamResultProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.QualificationExamResultProtocolId });
        builder.Metadata
            .FindNavigation(nameof(QualificationExamResultProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
