namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ScheduleMapping : EntityMapping
{
    public ScheduleMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Schedule";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Schedule>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ScheduleId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ScheduleId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Hours)
            .WithOne(e => e.Schedule)
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleId });
        builder.Metadata
            .FindNavigation(nameof(Schedule.Hours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Dates)
            .WithOne(e => e.Schedule)
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleId });
        builder.Metadata
            .FindNavigation(nameof(Schedule.Dates))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Lessons)
            .WithOne(e => e.Schedule)
            .HasForeignKey(e => new { e.SchoolYear, e.ScheduleId });
        builder.Metadata
            .FindNavigation(nameof(Schedule.Lessons))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ClassBook))
            .WithMany()
            .HasForeignKey(nameof(Schedule.SchoolYear), nameof(Schedule.ClassBookId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(Shift))
            .WithMany()
            .HasForeignKey(nameof(Schedule.SchoolYear), nameof(Schedule.ShiftId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
