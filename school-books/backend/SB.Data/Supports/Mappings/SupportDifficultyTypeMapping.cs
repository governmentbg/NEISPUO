namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportDifficultyTypeMapping : EntityMapping
{
    public SupportDifficultyTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SupportDifficultyType";

        var builder = modelBuilder.Entity<SupportDifficultyType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
