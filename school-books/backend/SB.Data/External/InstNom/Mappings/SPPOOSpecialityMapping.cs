namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SPPOOSpecialityMapping : EntityMapping
{
    public SPPOOSpecialityMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_nom";
        var tableName = "SPPOOSpeciality";

        var builder = modelBuilder.Entity<SPPOOSpeciality>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.SPPOOSpecialityId);
    }
}
