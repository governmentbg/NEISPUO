namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeChangeExamsAdmProtocolMapping : EntityMapping
{
    public GradeChangeExamsAdmProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeChangeExamsAdmProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<GradeChangeExamsAdmProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.GradeChangeExamsAdmProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.GradeChangeExamsAdmProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId });
        builder.Metadata
            .FindNavigation(nameof(GradeChangeExamsAdmProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.GradeChangeExamsAdmProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId });
        builder.Metadata
            .FindNavigation(nameof(GradeChangeExamsAdmProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
