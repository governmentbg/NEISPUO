namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class AttendanceMapping : EntityMapping
{
    public AttendanceMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Attendance";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Attendance>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.AttendanceId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.AttendanceId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasOne(typeof(PersonMedicalNotice))
            .WithMany()
            .HasForeignKey(nameof(Attendance.SchoolYear), nameof(Attendance.PersonId), nameof(Attendance.HisMedicalNoticeId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
