namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportDifficultyMapping : EntityMapping
{
    public SupportDifficultyMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SupportDifficulty";

        var builder = modelBuilder.Entity<SupportDifficulty>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SupportId, e.SupportDifficultyTypeId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
