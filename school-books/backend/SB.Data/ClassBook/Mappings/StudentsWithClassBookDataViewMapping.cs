namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class StudentsWithClassBookDataViewMapping : EntityMapping
{
    public StudentsWithClassBookDataViewMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<StudentsWithClassBookDataView>()
            .ToView("vwStudentsWithClassBookData", "school_books")
            .HasNoKey();
    }
}
