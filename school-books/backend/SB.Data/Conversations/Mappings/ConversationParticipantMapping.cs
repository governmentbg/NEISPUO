namespace SB.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class ConversationParticipantMapping : EntityMapping
{
    public ConversationParticipantMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ConversationParticipant";
        var sequenceName = $"{tableName}IdSequence";

        var builder = modelBuilder.Entity<ConversationParticipant>();
        modelBuilder.HasSequence<int>(sequenceName, schema);

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ConversationId, e.ConversationParticipantId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ParticipantType).HasColumnType("SMALLINT");

        builder.Property(e => e.ConversationParticipantId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        // Relationships
        builder.HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => new { cp.SchoolYear, cp.ConversationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.ParticipantGroup)
            .WithMany()
            .HasForeignKey(cp => new { cp.SchoolYear, cp.ConversationParticipantGroupId })
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation properties
        builder.HasOne<SysUser>()
            .WithMany()
            .HasForeignKey(cp => cp.SysUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
