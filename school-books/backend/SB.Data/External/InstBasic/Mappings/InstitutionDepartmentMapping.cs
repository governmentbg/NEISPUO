namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class InstitutionDepartmentMapping : EntityMapping
{
    public InstitutionDepartmentMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "inst_basic";
        var tableName = "InstitutionDepartment";

        var builder = modelBuilder.Entity<InstitutionDepartment>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.InstitutionDepartmentId);
    }
}
