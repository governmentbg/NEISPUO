using System.ComponentModel;

namespace MON.Shared.Enums
{
    public enum LodFinalizationActivity
    {
        [Description("Approved")]
        Approved = 1,

        [Description("UnApproved")]
        UnApproved = 2,

        [Description("Finalized")]
        Finalized = 3,

        [Description("UnFinalized")]
        UnFinalized = 4
    }
}
