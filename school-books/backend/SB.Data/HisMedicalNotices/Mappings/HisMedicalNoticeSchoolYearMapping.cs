namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class HisMedicalNoticeSchoolYearMapping : EntityMapping
{
    public HisMedicalNoticeSchoolYearMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "HisMedicalNoticeSchoolYear";

        var builder = modelBuilder.Entity<HisMedicalNoticeSchoolYear>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.HisMedicalNoticeId, e.SchoolYear });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
