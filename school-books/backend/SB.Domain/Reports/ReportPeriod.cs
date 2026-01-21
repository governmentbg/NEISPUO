namespace SB.Domain;

using System.ComponentModel;

public enum ReportPeriod
{
    [Description("Първи срок")]
    TermOne = 1,

    [Description("Втори срок")]
    TermTwo = 2,

    [Description("Цялата година")]
    WholeYear = 3,
}
