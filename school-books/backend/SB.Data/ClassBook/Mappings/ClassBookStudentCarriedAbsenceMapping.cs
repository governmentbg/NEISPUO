namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookStudentCarriedAbsenceMapping : EntityMapping
{
    public ClassBookStudentCarriedAbsenceMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookStudentCarriedAbsence";

        var builder = modelBuilder.Entity<ClassBookStudentCarriedAbsence>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
