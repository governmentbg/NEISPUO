namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookOffDayDateMapping : EntityMapping
{
    public ClassBookOffDayDateMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookOffDayDate";

        var builder = modelBuilder.Entity<ClassBookOffDayDate>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.Date });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.Date).HasColumnType("DATE");

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(ClassBook))
            .WithMany()
            .HasForeignKey(nameof(ClassBookOffDayDate.SchoolYear), nameof(ClassBookOffDayDate.ClassBookId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(typeof(OffDay))
            .WithMany()
            .HasForeignKey(nameof(ClassBookOffDayDate.SchoolYear), nameof(ClassBookOffDayDate.OffDayId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
