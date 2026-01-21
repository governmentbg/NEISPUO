namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SpbsBookRecordMovementMapping : EntityMapping
{
    public SpbsBookRecordMovementMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SpbsBookRecordMovement";

        var builder = modelBuilder.Entity<SpbsBookRecordMovement>();

        builder.ToTable(tableName, schema);
        builder.HasKey(e => new { e.SchoolYear, e.SpbsBookRecordId, e.OrderNum });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OrderNum).HasColumnType("SMALLINT");
    }
}
