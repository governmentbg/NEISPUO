namespace SB.Domain;

using System;

public class Sanction : IAggregateRoot
{
    // EF constructor
    private Sanction()
    {
        this.OrderNumber = null!;
        this.Description = null!;
        this.Version = null!;
    }

    public Sanction(
        int schoolYear,
        int classBookId,
        int personId,
        int sanctionTypeId,
        string orderNumber,
        DateTime orderDate,
        DateTime startDate,
        DateTime? endDate,
        string? description,
        string? cancelOrderNumber,
        DateTime? cancelOrderDate,
        string? cancelReason,
        string? ruoOrderNumber,
        DateTime? ruoOrderDate,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.SanctionTypeId = sanctionTypeId;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Description = description;
        this.CancelOrderNumber = cancelOrderNumber;
        this.CancelOrderDate = cancelOrderDate;
        this.CancelReason = cancelReason;
        this.RuoOrderNumber = ruoOrderNumber;
        this.RuoOrderDate = ruoOrderDate;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int SanctionId { get; private set; }
    public int ClassBookId { get; private set; }
    public int PersonId { get; private set; }
    public int SanctionTypeId { get; private set; }
    public string OrderNumber { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }
    public string? CancelOrderNumber { get; private set; }
    public DateTime? CancelOrderDate { get; private set; }
    public string? CancelReason { get; private set; }
    public string? RuoOrderNumber { get; private set; }
    public DateTime? RuoOrderDate { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    public void UpdateData(
        int sanctionTypeId,
        string orderNumber,
        DateTime orderDate,
        DateTime startDate,
        DateTime? endDate,
        string description,
        string? cancelOrderNumber,
        DateTime? cancelOrderDate,
        string? cancelReason,
        string? ruoOrderNumber,
        DateTime? ruoOrderDate,
        int modifiedBySysUserId)
    {
        this.SanctionTypeId = sanctionTypeId;
        this.OrderNumber = orderNumber;
        this.OrderDate = orderDate;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Description = description;
        this.CancelOrderNumber = cancelOrderNumber;
        this.CancelOrderDate = cancelOrderDate;
        this.CancelReason = cancelReason;
        this.RuoOrderNumber = ruoOrderNumber;
        this.RuoOrderDate = ruoOrderDate;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
