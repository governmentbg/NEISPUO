namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StudentMapping : EntityMapping
{
    public StudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "student";
        var tableName = "Student";

        var builder = modelBuilder.Entity<Student>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.PersonId);
    }
}
