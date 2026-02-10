namespace SB.Domain;

using System;

public class Person
{
    // EF constructor
    private Person()
    {
        this.FirstName = null!;
        this.LastName = null!;
        this.PersonalId = null!;
    }

    // only used properties should be mapped

    public int PersonId { get; private set; }

    public string FirstName { get; private set; }

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; }

    public DateTime? BirthDate { get; set; }

    public string PersonalId { get; set; }

    public int PersonalIdType { get; set; }

    public string? PermanentAddress { get; set; }

    public int? PermanentTownId { get; set; }

    public int? BirthPlaceTownId { get; set; }

    public int? BirthPlaceCountry { get; set; }

    public int? Gender { get; set; }
}
