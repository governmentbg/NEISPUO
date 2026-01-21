namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeMapping : EntityMapping
{
    public GradeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Grade";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Grade>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.GradeId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Ignore(e => e.GradeText);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasDiscriminator(e => e.Category)
            .HasValue<GradeDecimal>(GradeCategory.Decimal)
            .HasValue<GradeQualitative>(GradeCategory.Qualitative)
            .HasValue<GradeSpecialNeeds>(GradeCategory.SpecialNeeds);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ScheduleLesson))
            .WithMany()
            .HasForeignKey(nameof(Grade.SchoolYear), nameof(Grade.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(TeacherAbsenceHour))
            .WithMany()
            .HasForeignKey(nameof(Grade.SchoolYear), nameof(Grade.TeacherAbsenceId), nameof(Grade.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
