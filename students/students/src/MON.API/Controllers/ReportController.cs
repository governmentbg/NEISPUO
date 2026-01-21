using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Services.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace MON.API.Controllers
{
    /// <summary>
    /// Този контролер не може да наследи обичайния за проекта BaseApiController,
    /// защото Telerik reporting-ът изисква базовият клас да бъде ReportsControllerBase.
    /// Action-ите на ReportsControllerBase имат собствен routing.
    /// На ниво контролер трябва да има минимален route - в случая се използва името на наследника "/Reports".
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ReportController : ReportsControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportServiceConfiguration reportServiceConfiguration, IEmailService emailService, ILogger<ReportController> logger)
            : base(reportServiceConfiguration)
        {
            _emailService = emailService;
            _logger = logger;
        }

        protected override HttpStatusCode SendMailMessage(MailMessage mailMessage)
        {
            try
            {
                Task.FromResult(_emailService.SendEmailAsync(mailMessage));
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Грешка при изпращане на e-mail съобщение");
                return HttpStatusCode.InternalServerError;
            }

        }
    }
}
