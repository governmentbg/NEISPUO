namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GraduationThesisDefenseProtocolMapping : EntityMapping
{
    public GraduationThesisDefenseProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GraduationThesisDefenseProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<GraduationThesisDefenseProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GraduationThesisDefenseProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.GraduationThesisDefenseProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.GraduationThesisDefenseProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.GraduationThesisDefenseProtocolId });
        builder.Metadata
            .FindNavigation(nameof(StateExamsAdmProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
