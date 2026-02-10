namespace SB.Domain;

/// <summary>{{ClassBookCombineDO.Summary}}</summary>
public class ClassBookCombineDO
{
    /// <summary>{{ClassBookCombineDO.ParentClassId}}</summary>
    public int? ParentClassId { get; init; }

    /// <summary>{{ClassBookCombineDO.ChildClassIdForDataTransfer}}</summary>
    public int? ChildClassIdForDataTransfer { get; init; }
}
