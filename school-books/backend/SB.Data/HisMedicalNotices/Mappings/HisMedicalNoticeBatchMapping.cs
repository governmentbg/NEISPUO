namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HisMedicalNoticeBatchMapping : EntityMapping
{
    public HisMedicalNoticeBatchMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HisMedicalNoticeBatch";

        var builder = modelBuilder.Entity<HisMedicalNoticeBatch>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.HisMedicalNoticeBatchId);
        builder.Property(e => e.HisMedicalNoticeBatchId).UseIdentityColumn();

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
