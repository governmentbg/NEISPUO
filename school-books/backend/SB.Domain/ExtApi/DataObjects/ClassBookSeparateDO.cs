namespace SB.Domain;

/// <summary>{{ClassBookSeparateDO.Summary}}</summary>
public class ClassBookSeparateDO
{
    /// <summary>{{ClassBookSeparateDO.ParentClassId}}</summary>
    public int? ParentClassId { get; init; }

    /// <summary>{{ClassBookSeparateDO.ChildClassIds}}</summary>
    public int[]? ChildClassIds { get; init; }
}
