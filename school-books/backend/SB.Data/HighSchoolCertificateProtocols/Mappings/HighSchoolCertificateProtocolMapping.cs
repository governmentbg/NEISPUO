namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HighSchoolCertificateProtocolMapping : EntityMapping
{
    public HighSchoolCertificateProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HighSchoolCertificateProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<HighSchoolCertificateProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.HighSchoolCertificateProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.HighSchoolCertificateProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.HighSchoolCertificateProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.HighSchoolCertificateProtocolId });
        builder.Metadata
            .FindNavigation(nameof(StateExamsAdmProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.HighSchoolCertificateProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.HighSchoolCertificateProtocolId });
        builder.Metadata
            .FindNavigation(nameof(HighSchoolCertificateProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
