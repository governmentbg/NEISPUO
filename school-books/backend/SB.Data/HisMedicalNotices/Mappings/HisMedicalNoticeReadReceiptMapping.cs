namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HisMedicalNoticeReadReceiptMapping : EntityMapping
{
    public HisMedicalNoticeReadReceiptMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HisMedicalNoticeReadReceipt";

        var builder = modelBuilder.Entity<HisMedicalNoticeReadReceipt>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.ExtSystemId, e.HisMedicalNoticeId });

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Accesses)
            .WithOne(e => e.ReadReceipt)
            .HasForeignKey(e => new { e.ExtSystemId, e.HisMedicalNoticeId });
        builder.Metadata
            .FindNavigation(nameof(HisMedicalNoticeReadReceipt.Accesses))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(HisMedicalNotice))
            .WithMany()
            .HasForeignKey(nameof(HisMedicalNoticeReadReceipt.HisMedicalNoticeId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
