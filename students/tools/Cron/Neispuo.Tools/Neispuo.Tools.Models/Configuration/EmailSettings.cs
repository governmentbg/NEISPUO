namespace Neispuo.Tools.Models.Configuration
{
    public class EmailSettings
    {
        public bool Enabled { get; set; }
        public MailService Service { get; set; }
        public SmtpSettings Smtp { get; set; }
        public SendGridSettings SendGrid { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
    }

    public enum MailService
    {
        Smtp,
        SendGrid,
        None
    }

    public class SmtpSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Password { get; set; }
    }

    public class SendGridSettings
    {
        public string ApiKey { get; set; }
    }
}
