namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicDplrTeacherMapping : EntityMapping
{
    public TopicDplrTeacherMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TopicDplrTeacher";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<TopicDplrTeacher>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.TopicDplrId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(Person))
            .WithMany()
            .HasForeignKey(nameof(TopicDplrTeacher.PersonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
