namespace SB.Domain;
public record UpdateClassBookStudentCommandGradeless
{
    public int? CurriculumId { get; init; }
    public bool? WithoutFirstTermGrade { get; init; }
    public bool? WithoutSecondTermGrade { get; init; }
    public bool? WithoutFinalGrade { get; init; }
}
