namespace SB.Domain;

using System.ComponentModel;
using Common;

public enum SchoolTerm
{
    [Description("Първи срок")]
    [LocalizationKey("{{SchoolTerm.TermOne}}")]
    TermOne = 1,

    [Description("Втори срок")]
    [LocalizationKey("{{SchoolTerm.TermTwo}}")]
    TermTwo = 2,
}
