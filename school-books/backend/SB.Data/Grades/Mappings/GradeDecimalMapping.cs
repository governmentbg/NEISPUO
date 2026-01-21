namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeDecimalMapping : EntityMapping
{
    public GradeDecimalMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<GradeDecimal>();

        builder.Property(e => e.DecimalGrade).HasColumnType("DECIMAL(3,2)");
    }
}
