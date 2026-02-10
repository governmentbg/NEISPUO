namespace SB.Domain;

public record NotesModel(
    NotesModelNote[] Notes
);

public record NotesModelNote(
    string? Students,
    string? Description
);
