namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class LectureScheduleHourMapping : EntityMapping
{
    public LectureScheduleHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "LectureScheduleHour";

        var builder = modelBuilder.Entity<LectureScheduleHour>();

        builder.ToTable(tableName, schema);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.HasKey(e => new { e.SchoolYear, e.LectureScheduleId, e.ScheduleLessonId });
    }
}
