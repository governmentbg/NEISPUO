namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QueueMessageMapping : EntityMapping
{
    private int hiLoBlockSize;

    public QueueMessageMapping(IOptions<DataOptions> options)
        : base(options)
    {
        this.hiLoBlockSize = options.Value.HiLoBlockSize;
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QueueMessage";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<QueueMessage>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.Type, e.DueDateUnixTimeMs, e.QueueMessageId });

        builder.Property(e => e.QueueMessageId)
            .ForSqlServerUseSpGetRangeSequenceHiLo(
                sequenceName,
                schema,
                this.hiLoBlockSize);
        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
