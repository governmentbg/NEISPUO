namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Helpers.Mail;

internal class EmailSenderService : IEmailSenderService
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public EmailSenderService(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task SendEmailAsync(SendGridClient client, NotificationQueueMessage payload, CancellationToken ct)
    {
        var scope = this.serviceScopeFactory.CreateScope();
        var viewRender = scope.ServiceProvider.GetRequiredService<IViewRender>();

        var templateConfig = EmailTemplateConfig.Get(payload.MailTemplateName!);

        var from = new EmailAddress(templateConfig.Sender);
        var to = new EmailAddress(payload.RecipientEmail);

        var model = payload.Context!;

        var html = await viewRender.RenderAsync(
            $@"EmailSender\Templates\{templateConfig.TemplateFileName}",
            model);


        var mailMessage = MailHelper.CreateSingleEmail(from, to, templateConfig.MailSubject(model), string.Empty, html);

        await client.SendEmailAsync(mailMessage, ct);
    }
}
