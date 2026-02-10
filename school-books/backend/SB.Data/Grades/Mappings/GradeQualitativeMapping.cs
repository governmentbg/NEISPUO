namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class GradeQualitativeMapping : EntityMapping
{
    public GradeQualitativeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GradeQualitative>();
    }
}
