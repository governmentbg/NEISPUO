namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleLessonMapping : EntityMapping
{
    public ScheduleLessonMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ScheduleLesson";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ScheduleLesson>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleLessonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ScheduleLessonId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ScheduleDate))
            .WithMany()
            .HasForeignKey(nameof(ScheduleLesson.SchoolYear), nameof(ScheduleLesson.ScheduleId), nameof(ScheduleLesson.Date));
    }
}
