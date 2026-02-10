namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentTeacherSubjectsVO(
        string SubjectName,
        string SubjectTypeName,
        string TeacherName
    );
}
