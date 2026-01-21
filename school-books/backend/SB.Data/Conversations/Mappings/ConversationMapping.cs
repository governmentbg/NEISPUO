namespace SB.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class ConversationMapping : EntityMapping
{
    public ConversationMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Conversation";
        var sequenceName = $"{tableName}IdSequence";

        var builder = modelBuilder.Entity<Conversation>();

        modelBuilder.HasSequence<int>(sequenceName, schema);

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ConversationId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ConversationId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(c => c.Participants)
            .WithOne(c => c.Conversation)
            .HasForeignKey(c => new { c.SchoolYear, c.ConversationId, c.SysUserId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Messages)
            .WithOne(c => c.Conversation)
            .HasForeignKey(c => new { c.SchoolYear, c.ConversationId, c.ConversationMessageId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
