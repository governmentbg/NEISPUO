namespace SB.Domain;

using System;

public class RelocationDocument
{
    // EF constructor
    private RelocationDocument()
    {
        this.NoteNumber = null!;
    }

    // only used properties should be mapped

    public int Id { get; private set; }

    public int PersonId { get; private set; }

    public string NoteNumber { get; private set; }

    public DateTime NoteDate { get; private set; }
}
