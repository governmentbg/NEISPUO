namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GraduationThesisDefenseProtocolCommissionerMapping : EntityMapping
{
    public GraduationThesisDefenseProtocolCommissionerMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GraduationThesisDefenseProtocolCommissioner";

        var builder = modelBuilder.Entity<GraduationThesisDefenseProtocolCommissioner>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GraduationThesisDefenseProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OrderNum).HasColumnType("SMALLINT");
    }
}
