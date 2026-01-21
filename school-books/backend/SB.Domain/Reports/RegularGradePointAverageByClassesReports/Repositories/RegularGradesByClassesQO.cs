namespace SB.Domain;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Keyless]
public class RegularGradesByClassesQO
{
    public int ClassBookId { get; set; }

    public int? BasicClassId { get; set; }

    public required string ClassBookName { get; set; }

    public int StudentPersonId { get; set; }

    public int CurriculumId { get; set; }

    [Column(TypeName = "DECIMAL(3,2)")]
    public decimal Grade { get; set; }
}
