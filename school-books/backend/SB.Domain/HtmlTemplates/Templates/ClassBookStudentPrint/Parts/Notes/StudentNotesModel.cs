namespace SB.Domain;

using System;

public record StudentNotesModel(
    StudentNotesModelNote[] Notes
);

public record StudentNotesModelNote(
    DateTime DateCreated,
    string CreatedBySysUserName,
    string? Description
);
