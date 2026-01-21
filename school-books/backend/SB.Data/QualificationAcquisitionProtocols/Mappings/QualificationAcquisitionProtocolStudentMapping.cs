namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class QualificationAcquisitionProtocolStudentMapping : EntityMapping
{
    public QualificationAcquisitionProtocolStudentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "QualificationAcquisitionProtocolStudent";

        var builder = modelBuilder.Entity<QualificationAcquisitionProtocolStudent>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.QualificationAcquisitionProtocolId, e.ClassId, e.PersonId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.AverageDecimalGrade).HasColumnType("DECIMAL(3,2)");
        builder.Property(e => e.TheoryPoints).HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.PracticePoints).HasColumnType("DECIMAL(5,2)");
    }
}
