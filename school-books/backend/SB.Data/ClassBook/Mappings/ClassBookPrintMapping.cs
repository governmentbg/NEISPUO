namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class ClassBookPrintMapping : EntityMapping
{
    public ClassBookPrintMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "ClassBookPrint";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<ClassBookPrint>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.ClassBookId, e.ClassBookPrintId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.ClassBookPrintId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.HasMany(e => e.Signatures)
            .WithOne(e => e.Print)
            .HasForeignKey(e => new { e.SchoolYear, e.ClassBookId, e.ClassBookPrintId });
        builder.Metadata
            .FindNavigation(nameof(ClassBookPrint.Signatures))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // add relations for entities that do not reference each other
        builder.HasOne(typeof(SysUser))
            .WithMany()
            .HasForeignKey(nameof(ClassBookPrint.CreatedBySysUserId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
