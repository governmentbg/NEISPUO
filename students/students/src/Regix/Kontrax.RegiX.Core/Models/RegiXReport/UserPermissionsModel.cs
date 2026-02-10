namespace Kontrax.RegiX.Core.TestStandard.Models.RegiXReport
{
    public class UserPermissionsModel
    {

        public UserPermissionsModel(string userId, string displayName, bool isGlobalAdmin)
        {
            UserId = userId;
            DisplayName = displayName;
            IsGlobalAdmin = isGlobalAdmin;
        }

        public string UserId { get; }

        public string DisplayName { get; }

        public bool IsGlobalAdmin { get; }

        public RegiXReportPermissionModel RegiXReportIsAllowed(int regiXReportId, IdNameModel administration)
        {
            return new RegiXReportPermissionModel(true, $"Имате право да заявявате това удостоверение, тъй като сте супер.");
        }
    }
}
