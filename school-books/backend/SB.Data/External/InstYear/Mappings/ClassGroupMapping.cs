namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassGroupMapping : EntityMapping
{
    public ClassGroupMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_year";
        var tableName = "ClassGroup";

        var builder = modelBuilder.Entity<ClassGroup>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.ClassId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
