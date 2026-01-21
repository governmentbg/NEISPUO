namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ExamResultProtocolCommissionerMapping : EntityMapping
{
    public ExamResultProtocolCommissionerMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ExamResultProtocolCommissioner";

        var builder = modelBuilder.Entity<ExamResultProtocolCommissioner>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ExamResultProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OrderNum).HasColumnType("SMALLINT");
    }
}
