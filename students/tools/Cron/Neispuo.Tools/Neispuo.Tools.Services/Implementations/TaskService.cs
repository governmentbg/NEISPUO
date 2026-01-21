using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neispuo.Tools.DataAccess;
using Neispuo.Tools.Models;
using Neispuo.Tools.Models.Configuration;
using Neispuo.Tools.Services.Interfaces;
using Neispuo.Tools.Shared.Enums;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Neispuo.Tools.Services.Implementations
{
    public class TaskService : BaseService
    {
        protected readonly IEmailService _emailService;

        public TaskService(NeispuoContext context,
            ILogger<TaskService> logger, IEmailService emailService)
            : base(context, logger)
        {
            _emailService = emailService;
        }

        public async Task ProcessTasksAsync()
        {

            _logger.LogInformation("Обработка на необработени задачи");

            var tasks = await _context.ScheduledTasks
                .Where(t => t.StatusCode == "New")
                .ToListAsync();

            foreach (var task in tasks)
            {
                // Извличаме TaskType с включени TaskTypeNotificationOffsets
                var taskType = await _context.TaskTypes
                    .Include(tt => tt.TaskTypeNotificationOffsets)
                    .Include(tt => tt.NotificationTemplates)
                    .Where(tt => tt.Code == task.TaskTypeCode)
                    .FirstOrDefaultAsync();

                foreach (var offset in taskType.TaskTypeNotificationOffsets)
                {
                    var notifyTime = task.ScheduledTime.AddMinutes(offset.NotifyOffsetMinutes);

                    var notificationTemplate = taskType.NotificationTemplates.FirstOrDefault();
                    if (notificationTemplate == null)
                    {
                        _logger.LogWarning($"No NotificationTemplate found for TaskType Code {taskType.Code}");
                        continue; // Skip this offset if no template is available
                    }

                    var notification = new TaskNotification
                    {
                        ScheduledTaskId = task.Id,
                        NotifyOffsetMinutes = offset.NotifyOffsetMinutes,
                        NotifyTime = notifyTime,
                        NotificationTemplateId = notificationTemplate.Id,
                        Message = "",
                    };

                    task.TaskNotifications.Add(notification);
                }
                task.StatusCode = "Pending"; // Обновяваме статуса на задачата
            }

            await _context.SaveChangesAsync();

            //await _emailService.SendEmailAsync("plamen.ignatov@kontrax.bg",
            //                $"Обработка на необработени задачи",
            //                $"{DateTime.Now.ToString()}");
            await Task.CompletedTask;
        }

        public async Task ExecuteTasksAsync()
        {
            try
            {
                var tasks = await _context.TaskNotifications
                    .Include(tn => tn.ScheduledTask)
                    .Where(tn => !tn.IsSent)
                    .ToListAsync();

                if (tasks != null && tasks.Count > 0)
                {
                    foreach (var task in tasks)
                    {
                        if (task.ScheduledTask != null)
                        {

                            // Извличаме TaskType с включени TaskTypeNotificationOffsets
                            var taskType = await _context.TaskTypes
                                .Include(tt => tt.TaskTypeNotificationOffsets)
                                .Include(tt => tt.NotificationTemplates)
                                .Where(tt => tt.Code == task.ScheduledTask.TaskTypeCode)
                                .FirstOrDefaultAsync();

                            var notificationTemplate = taskType.NotificationTemplates.FirstOrDefault();

                            // Изпращане на съобщение
                            if (task.NotifyTime.ToUniversalTime() <= DateTime.Now.ToUniversalTime())
                            {
                                var bodyTemplate = Scriban.Template.Parse(notificationTemplate.BodyTemplate);
                                var titleTemplate = Scriban.Template.Parse(notificationTemplate.TitleTemplate);

                                var reader = new DictionaryViewReader(_context);
                                var data = await reader.ReadViewAsync(taskType.DataView, task.ScheduledTask.Payload);

                                foreach (var row in data.Take(3))
                                {
                                    var scriptObj = new Scriban.Runtime.ScriptObject();
                                    foreach (var kvp in row)
                                    {
                                        scriptObj.Add(kvp.Key, kvp.Value);
                                    }
                                    scriptObj.Add("taskScheduledTime", task.ScheduledTask.ScheduledTime.ToString("dd.MM.yyyy г. HH:mm"));
                                    scriptObj.Add("taskName", task.ScheduledTask.Name);
                                    scriptObj.Add("taskTypeName", taskType.Name);
                                    scriptObj.Add("taskTypeDescription", taskType.Description);

                                    var context = new TemplateContext();
                                    context.PushGlobal(scriptObj);

                                    var titleTemplateText = titleTemplate.Render(context);
                                    var bodyTemplateText = bodyTemplate.Render(context);

                                    Console.Out.WriteLine($"Sending email to {(string)row["email"]} with title: {titleTemplateText}");
                                    await _emailService.SendEmailAsync((string)row["email"],
                                                    titleTemplateText,
                                                    bodyTemplateText);
                                }
                                task.IsSent = true;
                                task.SentTime = DateTime.Now;
                                task.Message = $"Task executed at {DateTime.Now}";
                            }
                        }
                        else
                        {
                            // Задачата няма свързан ScheduledTask, проверяваме дали имаме получател под формата на Институция
                            if (task.InstitutionId != null)
                            {
                                if (task.NotifyTime <= DateTime.Now && !task.IsSent)
                                {
                                    await _emailService.SendEmailAsync("plamen.ignatov@kontrax.bg",
                                                    task.Title,
                                                    task.Body);

                                    task.IsSent = true;
                                    task.SentTime = DateTime.Now;
                                    task.Message = $"Task executed at {DateTime.Now}";
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                var scheduledPendingTasks = await
                    (from t in _context.ScheduledTasks
                     where t.StatusCode == "Pending"
                     select new
                     {
                         Id = t.Id,
                         SentTasks = t.TaskNotifications.Count(i => i.IsSent),
                         NotSentTasks = t.TaskNotifications.Count(i => !i.IsSent)
                     }).ToListAsync();

                if (scheduledPendingTasks == null || scheduledPendingTasks.Count > 0)
                {

                    foreach (var task in scheduledPendingTasks)
                    {
                        if (task.NotSentTasks == 0 && task.SentTasks > 0)
                        {
                            var scheduledTask = await _context.ScheduledTasks.FindAsync(task.Id);
                            if (scheduledTask != null)
                            {
                                scheduledTask.StatusCode = "Executed"; // Обновяваме статуса на задачата
                                scheduledTask.ExecutedTime = DateTime.Now;
                                _logger.LogInformation($"Scheduled Task {scheduledTask.Id} completed.");
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing tasks");
            }
            await Task.CompletedTask;
        }

        private async Task<InstitutionViewModel?> GetInstitutionViewModelAsync(int institutionId)
        {
            var institution = await _context.InstitutionSchoolYears
                .Where(i => i.InstitutionId == institutionId)
                .Select(i => new InstitutionViewModel
                {
                    Name = i.Name,
                    Email = $"{institutionId}@edu.mon.bg"
                })
                .FirstOrDefaultAsync();
            return institution;
        }
    }
}
