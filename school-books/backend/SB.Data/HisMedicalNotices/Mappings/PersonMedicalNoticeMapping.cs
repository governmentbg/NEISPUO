namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class PersonMedicalNoticeMapping : EntityMapping
{
    public PersonMedicalNoticeMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "PersonMedicalNotice";

        var builder = modelBuilder.Entity<PersonMedicalNotice>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.PersonId, e.HisMedicalNoticeId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
    }
}
