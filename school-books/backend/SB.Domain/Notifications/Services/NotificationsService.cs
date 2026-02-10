namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SB.Common;

class NotificationsService : INotificationsService
{
    private readonly ILogger<NotificationsService> logger;
    private readonly INotificationsQueryRepository notificationsQueryRepository;
    private readonly IQueueMessagesService queueMessagesService;
    private readonly ICommonCachedQueryStore commonCachedQueryStore;
    private readonly long? notificationTimeUnixMs;

    public NotificationsService(
        ILogger<NotificationsService> logger,
        IOptions<DomainOptions> domainOptions,
        INotificationsQueryRepository notificationsQueryRepository,
        IQueueMessagesService queueMessagesService,
        ICommonCachedQueryStore commonCachedQueryStore)
    {
        this.logger = logger;
        var notificationTime = domainOptions.Value.NotificationTime;
        this.notificationTimeUnixMs =
            notificationTime != null ?
                DateExtensions.GetDueDateUnixTimeMs(notificationTime.Value) :
                new DateTimeOffset(DateTime.Now.AddHours(1)).ToUnixTimeMilliseconds();
        this.notificationsQueryRepository = notificationsQueryRepository;
        this.queueMessagesService = queueMessagesService;
        this.commonCachedQueryStore = commonCachedQueryStore;
    }

    public async Task TryPostNotificationsAsync(string eventType, int studentPersonId, JObject jObject, CancellationToken ct)
    {
        try
        {
            var relativeEmails = await this.notificationsQueryRepository.GetStudentRelativeEmailsAsync(studentPersonId, ct);
            var studentInfo = await this.notificationsQueryRepository.GetStudentPersonInfoAsync(studentPersonId, ct);

            string pushNotificationTitle;
            string pushNotificationBody;
            StudentSettingsNotificationType emailNotificationType = default;
            StudentSettingsNotificationType pushNotificationType = default;
            var date = (DateTime?)jObject["date"] ?? default;

            switch (eventType)
            {
                case "NewGrade":
                    {
                        pushNotificationTitle = "Нова оценка";
                        pushNotificationBody = $"Нова оценка на {studentInfo.Name} по {(string)jObject["curriculumName"]!} на {date!.ToString("dd.MM.yyyy")}: {(string)jObject["gradeText"]!}";
                        emailNotificationType = StudentSettingsNotificationType.GradeEmail;
                        pushNotificationType = StudentSettingsNotificationType.GradePushNotification;
                        break;
                    }
                case "NewAbsence":
                    {
                        pushNotificationTitle = "Ново отсъствие";
                        pushNotificationBody = $"Ново {(string)jObject["absenceTypeText"]!} на {studentInfo.Name} по {(string)jObject["curriculumName"]!} на {date!.ToString("dd.MM.yyyy")}";
                        emailNotificationType = StudentSettingsNotificationType.AbsenceEmail;
                        pushNotificationType = StudentSettingsNotificationType.AbsencePushNotification;
                        break;
                    }
                case "NewAttendanceAbsence":
                    {
                        pushNotificationTitle = "Ново отсъствие";
                        pushNotificationBody = $"Ново отсъствие на {studentInfo.Name} на {date!.ToString("dd.MM.yyyy")}";
                        emailNotificationType = StudentSettingsNotificationType.AbsenceEmail;
                        pushNotificationType = StudentSettingsNotificationType.AbsencePushNotification;
                        break;
                    }
                case "NewRemark":
                    {
                        pushNotificationTitle = "Нов отзив";
                        pushNotificationBody = $"Нова {(string)jObject["remarkTypeText"]!} на {studentInfo.Name} по {(string)jObject["curriculumName"]!} на {date!.ToString("dd.MM.yyyy")}: {(string)jObject["note"]!}";
                        emailNotificationType = StudentSettingsNotificationType.RemarkEmail;
                        pushNotificationType = StudentSettingsNotificationType.RemarkPushNotification;
                        break;
                    }

                default:
                    throw new DomainException("Unknown eventType");
            }

            var pushSubscriptionUserIds = new List<int>();
            if (
                studentInfo.SysUserId != null &&
                await this.commonCachedQueryStore.ShouldSendNotificationAsync(
                        studentInfo.PersonId,
                        pushNotificationType,
                        ct))
            {
                pushSubscriptionUserIds.Add(studentInfo.SysUserId!.Value);
            }

            if (relativeEmails.Any())
            {
                jObject.Add("studentName", studentInfo.Name);

                var emails = new List<string>();
                foreach (var relativeInfo in relativeEmails)
                {
                    if (await this.commonCachedQueryStore
                            .ShouldSendNotificationAsync(
                                relativeInfo.PersonId,
                                emailNotificationType,
                                ct))
                    {
                        emails.Add(relativeInfo.Email);
                    }

                    if (await this.commonCachedQueryStore
                            .ShouldSendNotificationAsync(
                                relativeInfo.PersonId,
                                pushNotificationType,
                                ct))
                    {
                        pushSubscriptionUserIds.Add(relativeInfo.SysUserId);
                    }
                }

                if (emails.Any())
                {
                    var payloads =
                        emails
                            .Select(e =>
                                new NotificationQueueMessage(
                                    NotificationType.Email,
                                    null,
                                    e,
                                    eventType,
                                    null,
                                    null,
                                    jObject)
                            )
                            .ToArray();

                    await this.queueMessagesService.PostMessagesAndSaveAsync(
                        payloads,
                        this.notificationTimeUnixMs,
                        (string)jObject["emailTag"]!,
                        null,
                        ct);
                }
            }

            if (pushSubscriptionUserIds.Any())
            {
                var payloads = pushSubscriptionUserIds
                    .Distinct()
                    .Select(id => new NotificationQueueMessage(NotificationType.Push, id, null, null, pushNotificationTitle, pushNotificationBody, null))
                    .ToArray();

                await this.queueMessagesService.PostMessagesAndSaveAsync(
                    payloads,
                    this.notificationTimeUnixMs,
                    (string)jObject["pushNotificationTag"]!,
                    null,
                    ct);
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Error while trying to post notifications of type {eventType} for studentId {studentPersonId}", eventType, studentPersonId);
        }
    }

    public async Task TryPostNewMessageNotificationsAsync(int conversationId, int sysUserId, CancellationToken ct)
    {
        try
        {
            var participants =
                (await this.notificationsQueryRepository.GetParticipantsAsync(conversationId, ct))
                .Where(p => p.SysUserId != sysUserId)
                .ToArray();

            var emails = new Dictionary<int, string>();
            var pushSubscriptionUserIds = new List<int>();

            foreach (var participant in participants)
            {
                if (await this.commonCachedQueryStore
                        .ShouldSendNotificationAsync(
                            participant.PersonId,
                            StudentSettingsNotificationType.MessageEmail,
                            ct))
                {
                    emails.Add(participant.SysUserId, participant.Email);
                }

                if (await this.commonCachedQueryStore
                        .ShouldSendNotificationAsync(
                            participant.PersonId,
                            StudentSettingsNotificationType.MessagePushNotification,
                            ct))
                {
                    pushSubscriptionUserIds.Add(participant.SysUserId);
                }
            }

            if (emails.Any())
            {
                var payloads =
                    emails
                        .Select(e =>
                            new NotificationQueueMessage(
                                NotificationType.Email,
                                e.Key,
                                e.Value,
                                "NewMessage",
                                null,
                                null)
                        )
                        .ToArray();

                await this.queueMessagesService.PostMessagesAndSaveAsync(
                    payloads,
                    new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(),
                    null,
                    null,
                    ct);
            }

            if (pushSubscriptionUserIds.Any())
            {
                var payloads = pushSubscriptionUserIds
                    .Distinct()
                    .Select(id =>
                        new NotificationQueueMessage(
                            NotificationType.Push,
                            id,
                            null,
                            null,
                            "Ново съобщение",
                            "Имате ново съобщение в системата на НЕИСПУО."))
                    .ToArray();

                await this.queueMessagesService.PostMessagesAndSaveAsync(
                    payloads,
                    this.notificationTimeUnixMs,
                    null,
                    null,
                    ct);
            }

        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Error while trying to post notifications of type NewMessage for conversation {conversationId}", conversationId);
        }
    }
}
