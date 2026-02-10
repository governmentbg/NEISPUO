namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicPlanItemMapping : EntityMapping
{
    public TopicPlanItemMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TopicPlanItem";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<TopicPlanItem>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.TopicPlanItemId);
        builder.Property(e => e.TopicPlanItemId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(TopicPlan))
            .WithMany()
            .HasForeignKey(nameof(TopicPlanItem.TopicPlanId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
