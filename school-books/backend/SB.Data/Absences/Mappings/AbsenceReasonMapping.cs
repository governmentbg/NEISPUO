namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AbsenceReasonMapping : EntityMapping
{
    public AbsenceReasonMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "AbsenceReason";

        var builder = modelBuilder.Entity<AbsenceReason>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.Id);
    }
}
