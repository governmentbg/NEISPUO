namespace SB.Domain;

using System.ComponentModel;

public enum AdmProtocolType
{
    [Description("3-79 Протокол за допускане на ученици до държавни изпити")]
    StateExams = 1,

    [Description("3-79A Протокол за допускане на ученици до изпити за промяна оценката")]
    GradeChangeExams = 2
}
