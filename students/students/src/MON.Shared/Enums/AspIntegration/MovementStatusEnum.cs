namespace MON.Shared.Enums.AspIntegration
{
    using System.ComponentModel;

    public enum MovementStatusEnum
    {
        [Description("ЗАПИСАН")]
        Enrolled = 1,
        [Description("ПРЕМЕСТЕН")]
        Moved = 2,
        [Description("ОТПИСАН")]
        Discharged = 3
    }
}
