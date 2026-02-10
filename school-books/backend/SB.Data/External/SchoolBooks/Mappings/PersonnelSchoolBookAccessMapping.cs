namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PersonnelSchoolBookAccessMapping : EntityMapping
{
    public PersonnelSchoolBookAccessMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "PersonnelSchoolBookAccess";

        var builder = modelBuilder.Entity<PersonnelSchoolBookAccess>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.RowId);
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
