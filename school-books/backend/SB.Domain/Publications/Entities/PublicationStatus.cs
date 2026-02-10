namespace SB.Domain;

using System.ComponentModel;

public enum PublicationStatus
{
    [Description("Чернова")]
    Draft = 1,

    [Description("Публикувана")]
    Published = 2,

    [Description("Архивирана")]
    Archived = 3,
}
