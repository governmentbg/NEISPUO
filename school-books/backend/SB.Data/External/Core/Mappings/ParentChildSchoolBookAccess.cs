namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ParentChildSchoolBookAccessMapping : EntityMapping
{
    public ParentChildSchoolBookAccessMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "ParentChildSchoolBookAccess";

        var builder = modelBuilder.Entity<ParentChildSchoolBookAccess>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.ParentChildSchoolBookAccessId);
    }
}
