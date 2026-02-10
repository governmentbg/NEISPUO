namespace SB.Domain;

using System;

public class UserPushSubscription : IAggregateRoot
{
    // EF constructor
    private UserPushSubscription()
    {
        this.Endpoint = null!;
        this.P256dh = null!;
        this.Auth = null!;
    }

    public UserPushSubscription(
        int sysUserId,
        string endpoint,
        string p256dh,
        string auth)
    {

        this.SysUserId = sysUserId;
        this.Endpoint = endpoint;
        this.P256dh = p256dh;
        this.Auth = auth;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.ModifyDate = now;
    }

    public void UpdateData(
        string p256dh,
        string auth)
    {
        this.P256dh = p256dh;
        this.Auth = auth;

        this.ModifyDate = DateTime.Now;
    }

    public int UserPushSubscriptionId { get; private set; }

    public int SysUserId { get; private set; }

    public string Endpoint { get; private set; }

    public string P256dh { get; private set; }

    public string Auth { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public byte[] Version { get; private set; } = null!;
}

