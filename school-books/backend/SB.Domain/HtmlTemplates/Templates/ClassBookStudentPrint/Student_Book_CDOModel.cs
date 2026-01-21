namespace SB.Domain;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public record Student_Book_CDOModel(
    StudentCoverPageModel CoverPageModel,
    StudentAbsencesCdoModel AbsencesCdoModel,
    StudentNotesModel NotesModel
);
