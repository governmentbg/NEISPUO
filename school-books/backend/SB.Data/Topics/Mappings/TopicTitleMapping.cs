namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicTitleMapping : EntityMapping
{
    public TopicTitleMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TopicTitle";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<TopicTitle>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.TopicId, e.Index });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ClassBookTopicPlanItem))
            .WithMany()
            .HasForeignKey(nameof(TopicTitle.SchoolYear), nameof(TopicTitle.ClassBookTopicPlanItemId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
