namespace MON.Models.ASP
{
    using System.ComponentModel;

    public enum ASPFileStatusCheckEnum : int
    {
        [Description("В процес на импортиране")]
        InProgress = 0,
        [Description("Успешно импортиран")]
        Success = 1,
        [Description("Грешка")]
        Error = 2
    }
}
