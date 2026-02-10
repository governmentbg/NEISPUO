namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PersonalIdTypeMapping : EntityMapping
{
    public PersonalIdTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "noms";
        var tableName = "PersonalIDType";

        var builder = modelBuilder.Entity<PersonalIdType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.PersonalIdTypeId);
    }
}
