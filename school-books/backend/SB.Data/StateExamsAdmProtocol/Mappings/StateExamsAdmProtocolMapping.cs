namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StateExamsAdmProtocolMapping : EntityMapping
{
    public StateExamsAdmProtocolMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StateExamsAdmProtocol";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<StateExamsAdmProtocol>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.StateExamsAdmProtocolId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Commissioners)
            .WithOne(e => e.StateExamsAdmProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId });
        builder.Metadata
            .FindNavigation(nameof(StateExamsAdmProtocol.Commissioners))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Students)
            .WithOne(e => e.StateExamsAdmProtocol)
            .HasForeignKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId });
        builder.Metadata
            .FindNavigation(nameof(StateExamsAdmProtocol.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
