namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public class RegiXReportPermissionModel
    {
        public RegiXReportPermissionModel(bool? isAllowedForCurrentUser, string explanation)
        {
            IsAllowedForCurrentUser = isAllowedForCurrentUser;
            Explanation = explanation;
        }

        public bool? IsAllowedForCurrentUser { get; }

        public string Explanation { get; }
    }
}
