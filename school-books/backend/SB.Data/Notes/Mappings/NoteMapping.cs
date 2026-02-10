namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class NoteMapping : EntityMapping
{
    public NoteMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "school_books";
        var tableName = "Note";
        var sequenceName = $"{tableName}IdSequence";

        modelBuilder.HasSequence<int>(sequenceName, schema);

        var builder = modelBuilder.Entity<Note>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => new { e.SchoolYear, e.NoteId });
        builder.Property(e => e.SchoolYear).HasColumnType("SMALLINT");
        builder.Property(e => e.NoteId).ForSqlServerUseSpGetRangeSequenceHiLo(sequenceName, schema, this.HiLoBlockSize);

        builder.Property(e => e.Version)
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(e => e.Students)
          .WithOne(e => e.Note)
          .HasForeignKey(e => new { e.SchoolYear, e.NoteId });
        builder.Metadata
            .FindNavigation(nameof(Note.Students))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
