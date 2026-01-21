namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookMapping : EntityMapping
{
    public ClassBookMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBook";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ClassBook>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ClassBookId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);
        builder.Property(e => e.FullBookName).HasComputedColumnSql();

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.GradelessCurriculums)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.GradelessCurriculums))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.GradelessStudents)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.GradelessStudents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.SpecialNeedsStudents)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.SpecialNeedsStudents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.FirstGradeResultSpecialNeedsStudents)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.FirstGradeResultSpecialNeedsStudents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.StudentActivities)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.StudentActivities))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.StudentCarriedAbsences)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.StudentCarriedAbsences))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.Prints)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.Prints))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(e => e.StatusChanges)
            .WithOne(e => e.ClassBook)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId });
        builder.Metadata
            .FindNavigation(nameof(ClassBook.StatusChanges))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(InstitutionSchoolYear))
            .WithMany()
            .HasForeignKey(nameof(ClassBook.InstId), nameof(ClassBook.SchoolYear))
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(typeof(ClassGroup))
            .WithMany()
            .HasForeignKey(nameof(ClassBook.ClassId))
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(typeof(BasicClass))
            .WithMany()
            .HasForeignKey(nameof(ClassBook.BasicClassId))
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(typeof(SysUser))
            .WithMany()
            .HasForeignKey(nameof(ClassBook.CreatedBySysUserId))
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(typeof(SysUser))
            .WithMany()
            .HasForeignKey(nameof(ClassBook.ModifiedBySysUserId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
