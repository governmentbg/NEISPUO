namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System.ComponentModel.DataAnnotations.Schema;

internal static partial class ClassBooksQueryHelper
{
    [Keyless]
    public record ClassBooksForStudentsVO(
        [property: Column(TypeName = "SMALLINT")]int SchoolYear,
        int InstId,
        int ClassId,
        bool ClassIsLvl2,
        int ClassBookId,
        ClassBookType BookType,
        string BookName,
        string FullBookName,
        int? BasicClassId,
        bool IsValid,
        int PersonId,
        bool? IsIndividualCurriculum,
        StudentClassStatus PersonStatus,
        int? StudentSpecialityId);
}
