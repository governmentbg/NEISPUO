namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PersonMapping : EntityMapping
{
    public PersonMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "Person";

        var builder = modelBuilder.Entity<Person>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.PersonId);
    }
}
