using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neispuo.Tools.DataAccess;
using Neispuo.Tools.Models.Configuration;
using Neispuo.Tools.Services.Interfaces;
using Neispuo.Tools.Shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neispuo.Tools.Services.Implementations
{
    public class StatsService: BaseService, IStatsService
    {
        protected readonly IEmailService _emailService;

        public StatsService(NeispuoContext context,
            ILogger<StatsService> logger, IEmailService emailService)
            : base(context, logger)
        {
            _emailService = emailService;
        }

        public async Task GenerateCommonStatsAsync(int period)
        {
            _logger.LogInformation($"Генериране на общи справки за НЕИСПУО за последните {period} минути");
            var endOfPeriod = DateTime.Now;
            var startOfPeriod = endOfPeriod.AddMinutes(-period);

            int diplomas = _context.Diplomas.Where(i => i.CreateDate >= startOfPeriod).Count();
            int errors = _context.Logs.Where(i => i.TimeStamp >= startOfPeriod && i.Level == "Error" && i.AuditModuleId == (int)AuditModuleEnum.Students).Count();
            await _emailService.SendEmailAsync("plamen.ignatov@kontrax.bg",
                $"Статистика за последни {period} минути - от {startOfPeriod.ToString()} до {endOfPeriod.ToString()}",
                $"Брой дипломи: {diplomas}<br/>Брой грешки: {errors}");
        }
    }
}
