namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetStudentsDataVO(
        int? ClassNumber,
        int PersonId,
        string FirstName,
        string? MiddleName,
        string LastName,
        string PersonalId,
        DateTime? BirthDate,
        bool IsForeignBorn,
        string? BirthCountryName,
        string? BirthTownType,
        string? BirthTownName,
        string? AddressTownType,
        string? AddressTownName,
        string? AddressText,
        string? HomePhone,
        string? MobilePhone,
        string? DoctorName,
        string? DoctorPhone,
        DateTime? EnrollmentDate,
        DateTime? DischargeDate,
        string? EnrolledClassName,
        string Profession,
        GetStudentsDataVORelative[] Relatives,
        string? RelocationDocumentNoteNumber,
        DateTime? RelocationDocumentNoteDate,
        string? AdmissionDocumentNoteNumber,
        DateTime? AdmissionDocumentNoteDate,
        string? AdmissionReasonType,
        bool IsTransferred,
        GradelessCurriculum[] GradelessCurriculums
    );

    public record GetStudentsDataVORelative(
        string? FirstName,
        string? LastName,
        string? PhoneNumber
    );

    public record GradelessCurriculum(
        int Curriculum,
        bool WithoutFirstTermGrade,
        bool WithoutSecondTermGrade,
        bool WithoutFinalGrade
    );
}
