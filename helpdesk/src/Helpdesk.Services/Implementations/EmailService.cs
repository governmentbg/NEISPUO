namespace Helpdesk.Services.Implementations
{
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using Helpdesk.DataAccess;
    using Helpdesk.Models.Configuration;
    using Helpdesk.Services.Extensions;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Interfaces;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailService : BaseService, IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(HelpdeskContext context, IUserInfo userInfo, ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
            : base(context, userInfo, logger)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(System.Net.Mail.MailMessage message)
        {
            if (_emailSettings.Enabled)
            {
                switch (_emailSettings.Service)
                {
                    case MailService.Smtp:
                        {
                            try
                            {
                                var mimeMessage = new MimeMessage();
                                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                                foreach (var toAddress in message.To)
                                {
                                    mimeMessage.To.Add(new MailboxAddress(toAddress.DisplayName, toAddress.Address));
                                }
                                foreach (var ccAddress in message.CC)
                                {
                                    mimeMessage.Cc.Add(new MailboxAddress(ccAddress.DisplayName, ccAddress.Address));
                                }
                                foreach (var bccAddress in message.Bcc)
                                {
                                    mimeMessage.Bcc.Add(new MailboxAddress(bccAddress.DisplayName, bccAddress.Address));
                                }

                                mimeMessage.Subject = message.Subject;
                                var builder = new BodyBuilder();

                                if (message.IsBodyHtml)
                                {
                                    builder.HtmlBody = message.Body;
                                }
                                else
                                {
                                    // Set the plain-text version of the message text
                                    builder.TextBody = message.Body;
                                }

                                foreach (var attachment in message.Attachments)
                                {
                                    builder.Attachments.Add(attachment.Name, attachment.ContentStream, new ContentType(attachment.ContentType.MediaType, string.Empty));
                                }


                                // Now we just need to set the message body and we're done
                                mimeMessage.Body = builder.ToMessageBody();


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
                                throw new InvalidOperationException(ex.Message);
                            }
                        }
                        break;
                    case MailService.SendGrid:
                        try
                        {
                            var sendGridClient = new SendGridClient(_emailSettings.SendGrid.ApiKey);
                            var msg = message.GetSendGridMessage();
                            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                            msg.From = from;

                            var response = await sendGridClient.SendEmailAsync(msg);
                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError(await response.Body.ReadAsStringAsync());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Грешка при изпращане на e-mail през SendGrid");
                        }
                        break;
                }
            }
        }

        public async Task SendEmailAsync(string email, string subject, string message)
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
                            //if (_env.IsDevelopment())
                            //{
                            //    // The third parameter is useSSL (true if the client should make an SSL-wrapped
                            //    // connection to the server; otherwise, false).
                            //    await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, false);
                            //}
                            //else
                            //{
                            await client.ConnectAsync(_emailSettings.Smtp.MailServer);
                            //}

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
                            _logger.LogError(ex, "Грешка при изпращане на e-mail през SMTP");
                        }
                        break;
                    case MailService.SendGrid:
                        try
                        {
                            var sendGridClient = new SendGridClient(_emailSettings.SendGrid.ApiKey);
                            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                            var to = new EmailAddress(email);
                            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                            var response = await sendGridClient.SendEmailAsync(msg);
                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError(await response.Body.ReadAsStringAsync());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Грешка при изпращане на e-mail през SendGrid");
                        }
                        break;
                }
            }
        }

        public async Task SendEmailAsync(string email, string subject, string message, byte[] imageData, string imageName)
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
                            var builder = new BodyBuilder
                            {

                                // Set the plain-text version of the message text
                                TextBody = message
                            };

                            builder.Attachments.Add(imageName, imageData);

                            // Now we just need to set the message body and we're done
                            mimeMessage.Body = builder.ToMessageBody();


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
                            throw new InvalidOperationException(ex.Message);
                        }
                        break;
                    case MailService.SendGrid:
                        try
                        {
                            var sendGridClient = new SendGridClient(_emailSettings.SendGrid.ApiKey);
                            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                            var to = new EmailAddress(email);
                            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                            msg.AddAttachment(new Attachment() { Content = Convert.ToBase64String(imageData), Filename = imageName });
                            var response = await sendGridClient.SendEmailAsync(msg);
                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError(await response.Body.ReadAsStringAsync());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Грешка при изпращане на e-mail през SendGrid");
                        }
                        break;
                }
            }
        }
    }

}
