namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookCurriculumGradelessMapping : EntityMapping
{
    public ClassBookCurriculumGradelessMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookCurriculumGradeless";

        var builder = modelBuilder.Entity<ClassBookCurriculumGradeless>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.CurriculumId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
