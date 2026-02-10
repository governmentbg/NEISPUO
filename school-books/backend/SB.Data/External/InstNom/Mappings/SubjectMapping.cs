namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SubjectMapping : EntityMapping
{
    public SubjectMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "Subject";

        var builder = modelBuilder.Entity<Subject>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.SubjectId);
    }
}
