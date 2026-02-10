namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StudentClassMapping : EntityMapping
{
    public StudentClassMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "StudentClass";

        var builder = modelBuilder.Entity<StudentClass>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
