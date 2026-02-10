namespace SB.Domain;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using Lib.Net.Http.WebPush;

public class AngularPushNotification
{
    private const string WRAPPER_START = "{\"notification\":";
    private const string WRAPPER_END = "}";

    public class NotificationAction
    {
        public string Action { get; }

        public string Title { get; }

        public NotificationAction(string action, string title)
        {
            this.Action = action;
            this.Title = title;
        }
    }

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public IList<int> Vibrate { get; set; } = new List<int>();

    public IDictionary<string, object> Data { get; set; } = null!;

    public IList<NotificationAction> Actions { get; set; } = new List<NotificationAction>();

    public PushMessage ToPushMessage(string? topic = null, int? timeToLive = null, PushMessageUrgency urgency = PushMessageUrgency.Normal)
    {
        return new PushMessage(WRAPPER_START + JsonConvert.SerializeObject(this, _jsonSerializerSettings) + WRAPPER_END)
        {
            Topic = topic,
            TimeToLive = timeToLive,
            Urgency = urgency
        };
    }
}

