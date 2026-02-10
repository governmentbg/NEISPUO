namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ShiftHourMapping : EntityMapping
{
    public ShiftHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ShiftHour";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ShiftHour>();

        builder.ToTable(tableName, schema);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.HasKey(e => new { e.SchoolYear, e.ShiftId, e.Day, e.HourNumber });
    }
}
