namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HisMedicalNoticeMapping : EntityMapping
{
    public HisMedicalNoticeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HisMedicalNotice";

        var builder = modelBuilder.Entity<HisMedicalNotice>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.HisMedicalNoticeId);
        builder.Property(e => e.HisMedicalNoticeId).UseIdentityColumn();
        builder.Property(e => e.IdentifierType).HasColumnType("SMALLINT");

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.SchoolYears)
            .WithOne(e => e.HisMedicalNotice)
            .HasForeignKey(e => e.HisMedicalNoticeId);
        builder.Metadata
            .FindNavigation(nameof(HisMedicalNotice.SchoolYears))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(HisMedicalNoticeBatch))
            .WithMany()
            .HasForeignKey(nameof(HisMedicalNotice.HisMedicalNoticeBatchId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
