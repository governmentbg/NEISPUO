namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetTeacherSubjectsVO(
        string SubjectName,
        string SubjectTypeName,
        TeacherInfo[] Teachers
    );

    public record TeacherInfo(
        string Names,
        string? PhoneNumber
    )
    {
        public string GetNamesAndPhone()
            => this.PhoneNumber == null ? $"{this.Names}" : $"{this.Names} - {this.PhoneNumber}";

    };
}
