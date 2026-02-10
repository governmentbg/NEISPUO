namespace SB.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class ConversationParticipantGroupMapping : EntityMapping
{
    public ConversationParticipantGroupMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ConversationParticipantGroup";
        var sequenceName = $"{tableName}IdSequence";

        var builder = modelBuilder.Entity<ConversationParticipantGroup>();
        modelBuilder.HasSequence<int>(sequenceName, schema);

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ConversationParticipantGroupId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ParticipantType).HasColumnType("SMALLINT");

        builder.Property(e => e.ConversationParticipantGroupId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
    }
}
