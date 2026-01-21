namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StudentSettingsMappings : EntityMapping
{
    public StudentSettingsMappings(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "StudentSettings";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<StudentSettings>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.StudentSettingsId);
        builder.Property(e => e.StudentSettingsId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(Person))
            .WithMany()
            .HasForeignKey(nameof(StudentSettings.PersonId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
