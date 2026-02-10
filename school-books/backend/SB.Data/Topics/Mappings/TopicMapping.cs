namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicMapping : EntityMapping
{
    public TopicMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Topic";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Topic>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.TopicId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.TopicId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Titles)
            .WithOne(e => e.Topic)
            .HasForeignKey(e => new { e.SchoolYear, e.TopicId });
        builder.Metadata
            .FindNavigation(nameof(Topic.Titles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ScheduleLesson))
            .WithMany()
            .HasForeignKey(nameof(Topic.SchoolYear), nameof(Topic.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(TeacherAbsenceHour))
            .WithMany()
            .HasForeignKey(nameof(Topic.SchoolYear), nameof(Topic.TeacherAbsenceId), nameof(Topic.ScheduleLessonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
