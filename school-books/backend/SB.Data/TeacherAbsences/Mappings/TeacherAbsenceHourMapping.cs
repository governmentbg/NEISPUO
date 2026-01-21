namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TeacherAbsenceHourMapping : EntityMapping
{
    public TeacherAbsenceHourMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TeacherAbsenceHour";

        var builder = modelBuilder.Entity<TeacherAbsenceHour>();

        builder.ToTable(tableName, schema);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.HasKey(e => new { e.SchoolYear, e.TeacherAbsenceId, e.ScheduleLessonId });

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ScheduleLesson))
            .WithMany()
            .HasForeignKey(nameof(TeacherAbsenceHour.SchoolYear), nameof(TeacherAbsenceHour.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
