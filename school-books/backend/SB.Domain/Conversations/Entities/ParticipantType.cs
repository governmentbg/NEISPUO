namespace SB.Data;

using System.ComponentModel;

public enum ParticipantType : short
{
    [Description("Родители на клас")]
    ParentsForClass = 1,

    [Description("Учители на клас")]
    TeachersForClass = 2,

    [Description("родител")]
    Parent = 3,

    [Description("учител")]
    Teacher = 4,

    [Description("администратор")]
    Admin = 6
}
