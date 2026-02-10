namespace MON.API.Logging.Sinks
{
    using Serilog.Configuration;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog;
    using System;
    using MON.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using MON.Models.Configuration;
    using System.Configuration;
    using Microsoft.Extensions.Configuration;
    using SendGrid.Helpers.Mail;
    using MimeKit;
    using SendGrid;
    using MailKit.Net.Smtp;
    using System.Threading.Tasks;
    using Serilog.Formatting.Display;
    using System.IO;
    
    public class EmailSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string _toEmail;
        private readonly EmailSettings _emailSettings;
        private readonly MessageTemplateTextFormatter _formatter;

        public EmailSink(EmailSettings emailSettings, string toEmail, IFormatProvider formatProvider, string outputTemplate)
        {
            _formatProvider = formatProvider;
            _toEmail = toEmail;
            _emailSettings = emailSettings;
            _formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
        }

        public void Emit(LogEvent logEvent)
        {
            string title = logEvent.MessageTemplate.Text;
            using var writer = new StringWriter();
            _formatter.Format(logEvent, writer);
            var message = writer.ToString(); // logEvent.RenderMessage(_formatProvider);
            var logLevel = logEvent.Level switch
            {
                LogEventLevel.Fatal => "фатална грешка",
                LogEventLevel.Error => "грешка",
                LogEventLevel.Warning => "предупреждение",
                LogEventLevel.Information => "информация",
                LogEventLevel.Debug => "дебъг информация",
                LogEventLevel.Verbose => "подробна информация",
                _ => "информация"
            };

            Task.Run( async () =>
            {
                await SendEmailAsync(_toEmail, $"Настъпи {logLevel} в НЕИСПУО: {title}", $"{title}\r\n{message}");
            });
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            if (_emailSettings.Enabled)
            {
                switch (_emailSettings.Service)
                {
                    case MailService.Smtp:
                        try
                        {
                            var mimeMessage = new MimeMessage();
                            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                            mimeMessage.To.Add(new MailboxAddress(email, email));
                            mimeMessage.Subject = subject;
                            mimeMessage.Body = new TextPart("html")
                            {
                                Text = message
                            };

                            using var client = new SmtpClient();
                            await client.ConnectAsync(_emailSettings.Smtp.MailServer);

                            // Note: only needed if the SMTP server requires authentication
                            if (_emailSettings.Smtp.IsAuthenticated)
                            {
                                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Smtp.Password);
                            }

                            await client.SendAsync(mimeMessage);

                            await client.DisconnectAsync(true);

                        }
                        catch (Exception ex)
                        {
                            // TODO: handle exception better
                            throw new InvalidOperationException(ex.Message);
                        }
                        break;
                    case MailService.SendGrid:
                        var sendGridClient = new SendGridClient(_emailSettings.SendGrid.ApiKey);
                        var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                        var to = new EmailAddress(email);
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                        var response = await sendGridClient.SendEmailAsync(msg);
                        break;
                }
            }
        }
    }

    public static class EmailSinkExtensions
    {
        public static LoggerConfiguration EmailSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IConfiguration configuration,
                  string toEmail,
                  string outputTemplate,
                  LogEventLevel restrictedToMinimumLevel = LevelAlias.Maximum,
                  IFormatProvider formatProvider = null)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            return loggerConfiguration.Sink(new EmailSink(emailSettings, toEmail, formatProvider, outputTemplate), restrictedToMinimumLevel);
        }
    }
}
