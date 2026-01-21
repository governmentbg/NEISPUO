namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeChangeExamsAdmProtocolCommissionerMapping : EntityMapping
{
    public GradeChangeExamsAdmProtocolCommissionerMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeChangeExamsAdmProtocolCommissioner";

        var builder = modelBuilder.Entity<GradeChangeExamsAdmProtocolCommissioner>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OrderNum).HasColumnType("SMALLINT");
    }
}
