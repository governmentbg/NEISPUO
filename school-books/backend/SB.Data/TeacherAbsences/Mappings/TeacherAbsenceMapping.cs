namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class TeacherAbsenceMapping : EntityMapping
{
    public TeacherAbsenceMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "TeacherAbsence";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<TeacherAbsence>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.TeacherAbsenceId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.TeacherAbsenceId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Hours)
            .WithOne(e => e.TeacherAbsence)
            .HasForeignKey(e => new { e.SchoolYear, e.TeacherAbsenceId });
        builder.Metadata
            .FindNavigation(nameof(TeacherAbsence.Hours))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
