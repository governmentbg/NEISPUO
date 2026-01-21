namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StateExamsAdmProtocolStudentMapping : EntityMapping
{
    public StateExamsAdmProtocolStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StateExamsAdmProtocolStudent";

        var builder = modelBuilder.Entity<StateExamsAdmProtocolStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId, e.ClassId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");

        builder.HasMany(e => e.Subjects)
            .WithOne(e => e.StateExamsAdmProtocolStudent)
            .HasForeignKey(e => new { e.SchoolYear, e.StateExamsAdmProtocolId, e.ClassId, e.PersonId });
        builder.Metadata
            .FindNavigation(nameof(StateExamsAdmProtocolStudent.Subjects))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
