namespace MON.Services.Security.Permissions
{
    public class Permission
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Key { get; set; }
        public bool IsDeactivated { get; set; }
    }
}
