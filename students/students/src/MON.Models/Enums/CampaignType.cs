namespace MON.Models.Enums
{
    using System.ComponentModel;

    public enum CampaignType
    {
        [Description("Подаване на отсъствия")]
        Absence,
        [Description("АСП потвърждаване")]
        Asp
    }
}
