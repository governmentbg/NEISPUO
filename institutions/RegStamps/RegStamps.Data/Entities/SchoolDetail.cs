using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class SchoolDetail
{
    public int SchoolId { get; set; }

    public string? SchoolName { get; set; }

    public string? SchlMidName { get; set; }

    public string? SchoolType { get; set; }

    public string? SchoolTypeFin { get; set; }

    public string? BudgetFromName { get; set; }

    public string? BulstatNo { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public short? PostCode { get; set; }

    public string? Telephone1 { get; set; }

    public string? TelephoneF { get; set; }

    public string? EMail { get; set; }

    public int? CurrentYear { get; set; }

    public string? OblName { get; set; }

    public string? MunicipalityName { get; set; }
}
