namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PersonDetailMapping : EntityMapping
{
    public PersonDetailMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_basic";
        var tableName = "PersonDetail";

        var builder = modelBuilder.Entity<PersonDetail>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.PersonDetailId);
    }
}
