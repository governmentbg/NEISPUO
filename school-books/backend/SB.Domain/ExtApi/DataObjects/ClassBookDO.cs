namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{ClassBookDO.Summary}}</summary>
public class ClassBookDO
{
    /// <summary>{{ClassBookDO.ClassBookId}}</summary>
    public int? ClassBookId { get; init; }

    /// <summary>{{ClassBookDO.ClassId}}</summary>
    public int ClassId { get; init; }

    /// <summary>{{ClassBookDO.BookType}}</summary>
    public ClassBookType BookType { get; init; }

    /// <summary>{{ClassBookDO.BasicClassId}}</summary>
    public int? BasicClassId { get; init; }

    /// <summary>{{ClassBookDO.BookName}}</summary>
    [MaxLength(300)]
    public required string BookName { get; init; }

    /// <summary>{{ClassBookDO.IsFinalized}}</summary>
    public bool IsFinalized { get; init; }

    /// <summary>{{ClassBookDO.IsValid}}</summary>
    public bool IsValid { get; init; }

    /// <summary>{{ClassBookDO.InvalidationDate}}</summary>
    public DateTime? InvalidationDate { get; init; }
}
