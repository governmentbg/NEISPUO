namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum ApplicationStatusEnum
    {
        [Description("В процес на обработка")]
        InProcess = 0,
        [Description("Обработен")]
        Completed = 1,
        [Description("Анулиран")]
        Cancelled = 2,
        Assessment,
        [Description("Чернова")]
        Draft,
        [Description("Върнат за корекция")]
        ReturnedForCorrection,
        [Description("Подаден")]
        Submitted,
        [Description("Одобрен")]
        Approved,
        [Description("Отхвърлен")]
        Rejected,
        [Description("Отговор")]
        Response,
        [Description("Системно")]
        System,
        [Description("Отчетен период")]
        ReportingPeriod,
        [Description("Декласиран")]
        RankingRejected
    }
}
