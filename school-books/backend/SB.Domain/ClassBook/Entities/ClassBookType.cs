namespace SB.Domain;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using SB.Common;

public enum ClassBookType
{
    // Наредба No 8, Приложение No 2 към чл. 7, т. 14, Номенклатурен No 3-5
    [SuppressMessage("", "CA1707")]
    [Description("3-5 Дневник на група/подготвителна група")]
    [LocalizationKey("{{ClassBookType.Book_PG}}")]
    Book_PG = 1,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 15, Номенклатурен No 3-14
    [SuppressMessage("", "CA1707")]
    [Description("3-14 Дневник I - III клас")]
    [LocalizationKey("{{ClassBookType.Book_I_III}}")]
    Book_I_III = 2,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 16, Номенклатурен No 3-16
    [SuppressMessage("", "CA1707")]
    [Description("3-16 Дневник IV клас")]
    [LocalizationKey("{{ClassBookType.Book_IV}}")]
    Book_IV = 3,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 17, Номенклатурен No 3-87
    [SuppressMessage("", "CA1707")]
    [Description("3-87 Дневник V - XII клас")]
    [LocalizationKey("{{ClassBookType.Book_V_XII}}")]
    Book_V_XII = 4,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 18, Номенклатурен No 3-63
    [SuppressMessage("", "CA1707")]
    [Description("3-63 Дневник на група")]
    [LocalizationKey("{{ClassBookType.Book_CDO}}")]
    Book_CDO = 5,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 19, Номенклатурен No 3-63.1
    [SuppressMessage("", "CA1707")]
    [Description("3-63.1 Дневник за дейности за подкрепа за личностно развитие")]
    [LocalizationKey("{{ClassBookType.Book_DPLR")]
    Book_DPLR = 6,

    // Наредба No 8, Приложение No 2 към чл. 7, т. 20, Номенклатурен No 3-62
    [SuppressMessage("", "CA1707")]
    [Description("3-62 Дневник за група/паралелка в център за специална образователна подкрепа")]
    [LocalizationKey("{{ClassBookType.Book_CSOP}}")]
    Book_CSOP = 7,
}
