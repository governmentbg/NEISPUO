namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class EduFormMapping : EntityMapping
{
    public EduFormMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "EduForm";

        var builder = modelBuilder.Entity<EduForm>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.ClassEduFormId);
    }
}
