namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeChangeExamsAdmProtocolStudentSubjectMapping : EntityMapping
{
    public GradeChangeExamsAdmProtocolStudentSubjectMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeChangeExamsAdmProtocolStudentSubject";

        var builder = modelBuilder.Entity<GradeChangeExamsAdmProtocolStudentSubject>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeChangeExamsAdmProtocolId, e.ClassId, e.PersonId, e.SubjectId, e.SubjectTypeId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
