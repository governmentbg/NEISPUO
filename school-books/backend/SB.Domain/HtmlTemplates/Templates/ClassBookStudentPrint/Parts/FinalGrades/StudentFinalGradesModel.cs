namespace SB.Domain;

public record StudentFinalGradesModel(
   StudentFinalGradesModelItem[] Items
);

public record StudentFinalGradesModelItem(
   string CurriculumName,
   string? FirstTermGrade,
   string? SecondTermGrade,
   string? FinalGrade
);
