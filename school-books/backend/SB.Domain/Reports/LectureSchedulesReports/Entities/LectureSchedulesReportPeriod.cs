namespace SB.Domain;

using System.ComponentModel;

public enum LectureSchedulesReportPeriod
{
    [Description("За месец")]
    Month = 1,

    [Description("За първи срок")]
    TermOne = 2,

    [Description("За втори срок")]
    TermTwo = 3,

    [Description("За цялата година")]
    WholeYear = 4,
}
