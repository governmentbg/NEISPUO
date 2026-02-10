namespace SB.Domain;

public record ClassBookStudentPrintParams(
    int SchoolYear,
    int ClassBookId,
    int StudentPersonId);
