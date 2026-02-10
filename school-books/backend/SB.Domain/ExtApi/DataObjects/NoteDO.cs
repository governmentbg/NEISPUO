namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{NoteDO.Summary}}</summary>
public class NoteDO
{
    /// <summary>{{NoteDO.NoteId}}</summary>
    public int? NoteId { get; init; }

    /// <summary>{{NoteDO.Description}}</summary>
    [MaxLength(10000)]
    public required string Description { get; init; }

    /// <summary>{{NoteDO.IsForAllStudents}}</summary>
    public bool IsForAllStudents { get; init; }

    // TODO deprecated, remove nullability and make it required
    /// <summary>{{NoteDO.StudentIds}}</summary>
    public int[]? StudentIds { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    /// <remarks>
    /// Вместо този атрибут да се използва StudentIds !!!
    /// Оставено е за backwards compatibility. Да не се използва, ще бъде премахнато в бъдеще.
    /// Ученици, за които е бележката. Попълва се ако "IsForAllStudents = false"
    /// </remarks>
    [Obsolete("For removal")]
    public NoteStudentDO[]? Students { get; init; }
}

/// <summary>{{Common.Obsolete}}</summary>
[Obsolete("For removal")]
public class NoteStudentDO
{
    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{NoteStudentDO.PersonId}}</summary>
    public int PersonId { get; init; }
}
