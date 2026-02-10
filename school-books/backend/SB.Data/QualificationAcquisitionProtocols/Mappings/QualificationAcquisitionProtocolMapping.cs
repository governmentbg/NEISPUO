namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationAcquisitionProtocolMapping : EntityMapping
{
    public QualificationAcquisitionProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationAcquisitionProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<QualificationAcquisitionProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.QualificationAcquisitionProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.QualificationAcquisitionProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Students)
            .WithOne(e => e.QualificationAcquisitionProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.QualificationAcquisitionProtocolId });
        builder.Metadata
            .FindNavigation(nameof(QualificationAcquisitionProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.QualificationAcquisitionProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.QualificationAcquisitionProtocolId });
        builder.Metadata
            .FindNavigation(nameof(QualificationAcquisitionProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
