namespace SB.Data;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

internal static partial class ClassBooksQueryHelper
{
    [Keyless]
    public record StudentsForClassBookVO(
        [property: Column(TypeName = "SMALLINT")] int SchoolYear,
        int ClassBookClassId,
        int PersonId,
        bool IsTransferred,
        int? ClassNumber,
        int? StudentSpecialityId,
        bool? IsIndividualCurriculum,
        int? AdmissionDocumentId,
        int? RelocationDocumentId,
        DateTime EnrollmentDate,
        DateTime? DischargeDate);
}
