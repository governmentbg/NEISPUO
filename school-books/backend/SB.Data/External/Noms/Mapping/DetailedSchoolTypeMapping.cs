namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class DetailedSchoolTypeMapping : EntityMapping
{
    public DetailedSchoolTypeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "noms";
        var tableName = "DetailedSchoolType";

        var builder = modelBuilder.Entity<DetailedSchoolType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.DetailedSchoolTypeId);
    }
}
