namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TeacherCurriculumClassBooksViewMapping : EntityMapping
{
    public TeacherCurriculumClassBooksViewMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<TeacherCurriculumClassBooksView>()
            .ToView("vwTeacherCurriculumClassBooks", "school_books")
            .HasNoKey()
            .Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
