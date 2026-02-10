namespace SB.Domain;

public record UpdateGradeResultSessionsCommandSession(
   int? PersonId,
   int? CurriculumId,
   int? Session1Grade,
   bool? Session1NoShow,
   int? Session2Grade,
   bool? Session2NoShow,
   int? Session3Grade,
   bool? Session3NoShow);
