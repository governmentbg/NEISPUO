namespace SB.Data;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

class ConversationMessageMapping : EntityMapping
{
    public ConversationMessageMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ConversationMessage";
        var sequenceName = $"{tableName}IdSequence";

        var builder = modelBuilder.Entity<ConversationMessage>();

        modelBuilder.HasSequence<int>(sequenceName, schema);

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ConversationId, e.ConversationMessageId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.Property(e => e.ConversationMessageId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasOne(cm => cm.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(cm => new { cm.SchoolYear, cm.ConversationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
