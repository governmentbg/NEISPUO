namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicPlanPublisherMapping : EntityMapping
{
    public TopicPlanPublisherMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TopicPlanPublisher";

        var builder = modelBuilder.Entity<TopicPlanPublisher>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
