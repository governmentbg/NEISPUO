namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassTypeMappingMapping : EntityMapping
{
    public ClassTypeMappingMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "ClassType";

        var builder = modelBuilder.Entity<ClassType>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.ClassTypeId);
    }
}
