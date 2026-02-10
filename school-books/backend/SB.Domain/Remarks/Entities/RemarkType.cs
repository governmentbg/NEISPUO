namespace SB.Domain;

using System.ComponentModel;
using SB.Common;

public enum RemarkType
{
    [Description("Похвала")]
    [LocalizationKey("{{RemarkType.Good}}")]
    Good = 1,

    [Description("Забележка")]
    [LocalizationKey("{{RemarkType.Bad}}")]
    Bad = 2,
}
