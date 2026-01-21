namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookStudentSpecialNeedsMapping : EntityMapping
{
    public ClassBookStudentSpecialNeedsMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookStudentSpecialNeeds";

        var builder = modelBuilder.Entity<ClassBookStudentSpecialNeeds>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.PersonId, e.CurriculumId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
