namespace SB.Domain;

using System.ComponentModel.DataAnnotations;

/// <summary>{{StudentActivitiesDO.Summary}}</summary>
public class StudentActivitiesDO
{
    /// <summary>{{StudentActivitiesDO.PersonId}}</summary>
    public int? PersonId { get; init; }

    /// <summary>{{StudentActivitiesDO.Activities}}</summary>
    [MaxLength(1000)]
    public string? Activities { get; init; }
}
