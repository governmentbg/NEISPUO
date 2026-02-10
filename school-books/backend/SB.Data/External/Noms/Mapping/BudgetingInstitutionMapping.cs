namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class BudgetingInstitutionMapping : EntityMapping
{
    public BudgetingInstitutionMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "noms";
        var tableName = "BudgetingInstitution";

        var builder = modelBuilder.Entity<BudgetingInstitution>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.BudgetingInstitutionId);
    }
}
