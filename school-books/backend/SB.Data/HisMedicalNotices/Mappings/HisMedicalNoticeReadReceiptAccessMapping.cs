namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HisMedicalNoticeReadReceiptAccessMapping : EntityMapping
{
    public HisMedicalNoticeReadReceiptAccessMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HisMedicalNoticeReadReceiptAccess";

        var builder = modelBuilder.Entity<HisMedicalNoticeReadReceiptAccess>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.ExtSystemId, e.HisMedicalNoticeId, e.SchoolYear, e.InstId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ClassBookExtProvider))
            .WithMany()
            .HasForeignKey(nameof(HisMedicalNoticeReadReceiptAccess.SchoolYear), nameof(HisMedicalNoticeReadReceiptAccess.InstId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
