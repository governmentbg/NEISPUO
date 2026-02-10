using System;

namespace MON.Models.UserManagement
{
    public class UserManagementIntegrationResult
    {
        public string Action { get; set; }

        public bool IsError { get; set; }

        public string Creator { get; set; }

        public DateTime Timestamp { get; set; }

        public string Payload { get; set; }
        public string Message { get; set; }

        public string ResponseMessage { get; set; }
    }
}
