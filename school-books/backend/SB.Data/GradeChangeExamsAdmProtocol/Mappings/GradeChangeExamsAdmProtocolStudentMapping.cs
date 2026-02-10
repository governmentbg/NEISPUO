namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeChangeExamsAdmProtocolStudentMapping : EntityMapping
{
    public GradeChangeExamsAdmProtocolStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeChangeExamsAdmProtocolStudent";

        var builder = modelBuilder.Entity<GradeChangeExamsAdmProtocolStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId, e.ClassId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.HasMany(e => e.Subjects)
            .WithOne(e => e.GradeChangeExamsAdmProtocolStudent)
            .HasForeignKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId, e.ClassId, e.PersonId });
        builder.Metadata
            .FindNavigation(nameof(GradeChangeExamsAdmProtocolStudent.Subjects))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
