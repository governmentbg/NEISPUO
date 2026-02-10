namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class LectureScheduleMapping : EntityMapping
{
    public LectureScheduleMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "LectureSchedule";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<LectureSchedule>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.LectureScheduleId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.LectureScheduleId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Hours)
            .WithOne(e => e.LectureSchedule)
            .HasForeignKey(e => new { e.SchoolYear, e.LectureScheduleId });
        builder.Metadata
            .FindNavigation(nameof(LectureSchedule.Hours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
