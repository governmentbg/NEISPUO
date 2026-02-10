namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{SupportDO.Summary}}</summary>
public class SupportDO
{
    /// <summary>{{SupportDO.SupportId}}</summary>
    public int? SupportId { get; init; }

    /// <summary>{{SupportDO.Description}}</summary>
    [MaxLength(10000)]
    public string? Description { get; init; }

    /// <summary>{{SupportDO.ExpectedResult}}</summary>
    [MaxLength(10000)]
    public string? ExpectedResult { get; init; }

    /// <summary>{{SupportDO.EndDate}}</summary>
    public DateTime EndDate { get; init; }

    /// <summary>{{SupportDO.SupportDifficultyTypeIds}}</summary>
    public int[] SupportDifficultyTypeIds { get; init; } = Array.Empty<int>();

    /// <summary>{{SupportDO.TeacherIds}}</summary>
    public int[] TeacherIds { get; init; } = Array.Empty<int>();

    /// <summary>{{SupportDO.IsForAllStudents}}</summary>
    public bool IsForAllStudents { get; init; }

    // TODO deprecated, remove nullability and make it required
    /// <summary>{{SupportDO.StudentIds}}</summary>
    public int[]? StudentIds { get; init; }

    /// <summary>{{SupportDO.Activities}}</summary>
    public SupportActivityDO[] Activities { get; init; } = Array.Empty<SupportActivityDO>();

    /// <summary>{{Common.Obsolete}}</summary>
    /// <remarks>Вместо този атрибут да се използва StudentIds !!!
    /// Оставено е за backwards compatibility. Да не се използва, ще бъде премахнато в бъдеще.
    /// Ученици, за които е подкрепата. Попълва се ако "IsForAllStudents = false"
    /// </remarks>
    [Obsolete("For removal")]
    public SupportStudentDO[]? Students { get; init; }
}

/// <summary>{{Common.Obsolete}}</summary>
[Obsolete("For removal")]
public class SupportStudentDO
{
    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{SupportStudentDO.PersonId}}</summary>
    public int PersonId { get; init; }
}

/// <summary>{{SupportActivityDO.Summary}}</summary>
public class SupportActivityDO
{
    /// <summary>{{SupportActivityDO.SupportActivityTypeId}}</summary>
    public int SupportActivityTypeId { get; init; }

    /// <summary>{{SupportActivityDO.Target}}</summary>
    [MaxLength(10000)]
    public string? Target { get; init; }

    /// <summary>{{SupportActivityDO.Result}}</summary>
    [MaxLength(10000)]
    public string? Result { get; init; }

    /// <summary>{{SupportActivityDO.Date}}</summary>
    public DateTime? Date { get; init; }
}
