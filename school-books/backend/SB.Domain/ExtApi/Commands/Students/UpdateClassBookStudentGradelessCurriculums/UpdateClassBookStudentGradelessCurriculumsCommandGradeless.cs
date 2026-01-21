namespace SB.Domain;

public record UpdateClassBookStudentGradelessCurriculumsCommandGradeless
{
    public int CurriculumId { get; init; }
    public bool WithoutFirstTermGrade { get; init; }
    public bool WithoutSecondTermGrade { get; init; }
    public bool WithoutFinalGrade { get; init; }
}
