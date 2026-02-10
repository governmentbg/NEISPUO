namespace SB.Domain;

public record UpdateGradeResultsCommandStudent(
   int? PersonId,
   GradeResultType? InitialResultType,
   int[] RetakeExamCurriculumIds,
   GradeResultType? FinalResultType);
