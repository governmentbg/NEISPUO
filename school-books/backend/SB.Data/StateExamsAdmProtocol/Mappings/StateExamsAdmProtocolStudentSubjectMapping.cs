namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StateExamsAdmProtocolStudentSubjectMapping : EntityMapping
{
    public StateExamsAdmProtocolStudentSubjectMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StateExamsAdmProtocolStudentSubject";

        var builder = modelBuilder.Entity<StateExamsAdmProtocolStudentSubject>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId, e.ClassId, e.PersonId, e.SubjectId, e.SubjectTypeId, e.IsAdditional });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
