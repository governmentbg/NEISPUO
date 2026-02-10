namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeResultSubjectMapping : EntityMapping
{
    public GradeResultSubjectMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "GradeResultSubject";

        var builder = modelBuilder.Entity<GradeResultSubject>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.GradeResultId, e.CurriculumId });

        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
