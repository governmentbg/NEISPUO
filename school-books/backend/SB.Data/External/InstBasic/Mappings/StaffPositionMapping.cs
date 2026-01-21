namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StaffPositionMapping : EntityMapping
{
    public StaffPositionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_basic";
        var tableName = "StaffPosition";

        var builder = modelBuilder.Entity<StaffPosition>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.StaffPositionId, e.PersonId });
    }
}
