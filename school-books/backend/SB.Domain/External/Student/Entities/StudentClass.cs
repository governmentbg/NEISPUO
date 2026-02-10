namespace SB.Domain;

using System;

public class StudentClass
{
    // EF constructor
    private StudentClass()
    {
    }

    // only used properties should be mapped

    public int Id { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }

    public int? StudentSpecialityId { get; private set; }

    public int? ClassNumber { get; private set; }

    public DateTime EnrollmentDate { get; private set; }

    public DateTime? DischargeDate { get; private set; }

    public int? BasicClassId { get; private set; }

    public StudentClassStatus Status { get; private set; }

    public bool? IsIndividualCurriculum { get; private set; }

    public bool? IsNotPresentForm { get; private set; }

    public int? AdmissionDocumentId { get; private set; }

    public int? RelocationDocumentId { get; private set; }

    public int InstitutionId { get; private set; }

    public void SetClassNumber(int? classNumber)
    {
        this.ClassNumber = classNumber;
    }
}
