namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookStudentPrintMapping : EntityMapping
{
    public ClassBookStudentPrintMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookStudentPrint";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ClassBookStudentPrint>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.ClassBookStudentPrintId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ClassBookStudentPrintId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(SysUser))
            .WithMany()
            .HasForeignKey(nameof(ClassBookStudentPrint.CreatedBySysUserId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
