namespace SB.Domain;

using System.ComponentModel;

public enum PublicationType
{
    [Description("Вътрешна")]
    Internal = 1,

    [Description("Публична")]
    Public = 2,
}
