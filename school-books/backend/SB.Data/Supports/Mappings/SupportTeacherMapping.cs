namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SupportTeacherMapping : EntityMapping
{
    public SupportTeacherMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "SupportTeacher";

        var builder = modelBuilder.Entity<SupportTeacher>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.SupportId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
