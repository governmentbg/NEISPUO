namespace SB.Domain;

using System;

/// <summary>{{GradeResultDO.Summary}}</summary>
public class GradeResultDO
{
    /// <summary>{{GradeResultDO.GradeResultId}}</summary>
    public int? GradeResultId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{GradeResultDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{GradeResultDO.InitialResultType}}</summary>
    public GradeResultType InitialResultType { get; init; }

    /// <summary>{{GradeResultDO.RetakeExamCurriculums}}</summary>
    public GradeResultCurriculumDO[] RetakeExamCurriculums { get; init; } = Array.Empty<GradeResultCurriculumDO>();

    /// <summary>{{GradeResultDO.FinalResultType}}</summary>
    public GradeResultType? FinalResultType { get; init; }
}

/// <summary>{{GradeResultCurriculumDO.Summary}}</summary>
public class GradeResultCurriculumDO
{
    /// <summary>{{GradeResultCurriculumDO.CurriculumId}}</summary>
    public int CurriculumId { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session1Grade}}</summary>
    public int? Session1Grade { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session1NoShow}}</summary>
    public bool? Session1NoShow { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session2Grade}}</summary>
    public int? Session2Grade { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session2NoShow}}</summary>
    public bool? Session2NoShow { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session3Grade}}</summary>
    public int? Session3Grade { get; init; }

    /// <summary>{{GradeResultCurriculumDO.Session3NoShow}}</summary>
    public bool? Session3NoShow { get; init; }
}
