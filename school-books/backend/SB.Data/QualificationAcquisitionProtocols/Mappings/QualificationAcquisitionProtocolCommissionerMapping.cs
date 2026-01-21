namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationAcquisitionProtocolCommissionerMapping : EntityMapping
{
    public QualificationAcquisitionProtocolCommissionerMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationAcquisitionProtocolCommissioner";

        var builder = modelBuilder.Entity<QualificationAcquisitionProtocolCommissioner>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.QualificationAcquisitionProtocolId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.OrderNum).HasColumnType("SMALLINT");
    }
}
