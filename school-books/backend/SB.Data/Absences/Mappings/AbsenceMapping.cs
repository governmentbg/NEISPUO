namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AbsenceMapping : EntityMapping
{
    public AbsenceMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Absence";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Absence>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.AbsenceId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.AbsenceId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ScheduleLesson))
            .WithMany()
            .HasForeignKey(nameof(Absence.SchoolYear), nameof(Absence.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(TeacherAbsenceHour))
            .WithMany()
            .HasForeignKey(nameof(Absence.SchoolYear), nameof(Absence.TeacherAbsenceId), nameof(Absence.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(PersonMedicalNotice))
            .WithMany()
            .HasForeignKey(nameof(Absence.SchoolYear), nameof(Absence.PersonId), nameof(Absence.HisMedicalNoticeId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
