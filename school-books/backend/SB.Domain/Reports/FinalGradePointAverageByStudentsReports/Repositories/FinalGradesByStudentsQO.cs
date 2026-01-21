namespace SB.Domain;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Keyless]
public class FinalGradesByStudentsQO
{
    public required string ClassBookName { get; set; }

    public required string StudentName { get; set; }

    public required bool IsTransferred { get; set; }

    [Column(TypeName = "DECIMAL(3,2)")]
    public decimal FinalGradePointAverage { get; set; }
}
