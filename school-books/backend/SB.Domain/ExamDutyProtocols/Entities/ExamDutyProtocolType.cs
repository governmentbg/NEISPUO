namespace SB.Domain;

using System.ComponentModel;

public enum ExamDutyProtocolType
{
    [Description("3-82 Протокол за дежурство при провеждане на изпит")]
    Exam = 1,

    [Description("3-82 ДЗИ Протокол за дежурство при провеждане на държавен зрелостен изпит")]
    State = 2,

    [Description("3-82 Протокол за дежурство при провеждане на изпит за проверка на способностите")]
    SkillsCheck = 3,

    [Description("3-82 Протокол за дежурство при провеждане на писмен изпит от НВО")]
    Nvo = 4,
}
