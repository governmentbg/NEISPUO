namespace SB.Domain;

using System;

public record StudentsModel(
    bool IsPg,
    StudentsModelStudent[] Students
);

public record StudentsModelStudent(
    int? ClassNumber,
    string FullName,
    DateTime? BirthDate,
    string? PersonalId,
    string? BirthPlace,
    string? AddressAndPhoneNumber,
    string? DoctorNameAndPhoneNumber,
    DateTime? EnrollmentDate,
    DateTime? DischargeDate,
    string? EnrolledClassName,
    string? Profession,
    string[] ParentsContact,
    string? RelocationDocumentNoteNumber,
    DateTime? RelocationDocumentNoteDate,
    string? AdmissionDocumentNoteNumber,
    DateTime? AdmissionDocumentNoteDate,
    string? AdmissionReasonType,
    bool IsTransferred,
    string? TeacherNames = null
);
