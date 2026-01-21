namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeSpecialNeedsMapping : EntityMapping
{
    public GradeSpecialNeedsMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GradeSpecialNeeds>();
    }
}
