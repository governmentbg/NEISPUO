namespace SB.Domain;

using System.ComponentModel;

public enum ClassBookTypeError
{
    [Description("паралелката/групата е от неизвестен вид")]
    UnknownClassKind,

    [Description("паралелката/групата е с невалиден випуск (клас)")]
    InvalidBasicClass,

    [Description("паралелката/групата е без випуск (клас)")]
    Lvl1RegularOrLvl2CombinedMissingBasicClass,

    [Description("паралелката/групата е слята/разновъзрастова, но няма подгрупи")]
    Lvl1CombinedHasNoChildClassGroups,

    [Description("дневниците на подгрупите са различни")]
    NoCommonClassBookType,

    [Description("паралелката/групата е подгрупа, която не е маркирана като слята/разновъзрастова в СО")]
    IsLvl2Regular,
}
