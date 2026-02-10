namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class UserPushSubscriptionMapping : EntityMapping
{
    public UserPushSubscriptionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "UserPushSubscription";

        var builder = modelBuilder.Entity<UserPushSubscription>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.UserPushSubscriptionId);
        builder.Property(e => e.UserPushSubscriptionId).UseIdentityColumn();

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
