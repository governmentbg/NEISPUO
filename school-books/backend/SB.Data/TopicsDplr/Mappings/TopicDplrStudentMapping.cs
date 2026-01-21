namespace SB.Data;

using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TopicDplrStudentMapping : EntityMapping
{
    public TopicDplrStudentMapping(IOptions<DataOptions> options) : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TopicDplrStudent";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<TopicDplrStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.TopicDplrId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(Person))
            .WithMany()
            .HasForeignKey(nameof(TopicTeacher.PersonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
