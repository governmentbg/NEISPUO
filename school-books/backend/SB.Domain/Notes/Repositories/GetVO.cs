namespace SB.Domain;

using System.Text.Json.Serialization;

public partial interface INotesQueryRepository
{
    public record GetVO(
        int NoteId,
        int[] StudentIds,
        string? Description,
        bool IsForAllStudents,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasCreatorAccess { get; set; } // should be mutable
    }
}
