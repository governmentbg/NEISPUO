namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class CurriculumTeacherMapping : EntityMapping
{
    public CurriculumTeacherMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_year";
        var tableName = "CurriculumTeacher";

        var builder = modelBuilder.Entity<CurriculumTeacher>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.CurriculumId, e.StaffPositionId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
